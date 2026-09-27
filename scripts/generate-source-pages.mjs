import { readdir, readFile, writeFile, mkdir } from 'node:fs/promises'
import path from 'node:path'

const root = path.resolve('sample')
const output = path.resolve('docs/source')
const skipDirectories = new Set(['bin', 'obj', 'node_modules', 'dist', '.angular', '.vs', '.vscode', '.git'])
const skipFiles = new Set([
  'EmployeeManagement.Api.csproj.user', 'favicon.ico', '.gitignore', '.editorconfig',
  '.prettierrc', 'package-lock.json', 'dotnet-tools.json', 'launchSettings.json',
  'AppDbContextModelSnapshot.cs'
])
const allowedExtensions = new Set(['.cs', '.cshtml', '.ts', '.html', '.scss', '.css', '.json', '.csproj', '.js'])

const pages = {
  'api-project': 'API project and startup',
  'api-contracts': 'API entities and contracts',
  'api-data': 'API EF Core and migration',
  'api-services': 'API controllers and services',
  'angular-project': 'Angular project and bootstrap',
  'angular-core': 'Angular models and core services',
  'angular-auth': 'Angular login and registration',
  'angular-employees': 'Angular employee screens',
  'angular-extra': 'Other Angular files',
  'angular-tests': 'Angular tests',
  'mvc-project': 'MVC project and startup',
  'mvc-data': 'MVC models, view models, and migration',
  'mvc-controller': 'MVC controllers',
  'mvc-views': 'MVC Razor views and assets',
}

async function walk(directory) {
  const result = []
  for (const entry of await readdir(directory, { withFileTypes: true })) {
    const fullPath = path.join(directory, entry.name)
    if (entry.isDirectory()) {
      if (!skipDirectories.has(entry.name)) result.push(...await walk(fullPath))
    } else if (entry.isFile() && !skipFiles.has(entry.name) && !entry.name.endsWith('.Designer.cs') &&
      allowedExtensions.has(path.extname(entry.name)) &&
      !fullPath.includes(`${path.sep}wwwroot${path.sep}lib${path.sep}`)) {
      result.push(fullPath)
    }
  }
  return result
}

function pageFor(relative) {
  const parts = relative.split('/')
  const project = parts[1]
  const remainder = parts.slice(2).join('/')
  if (project === 'EmployeeManagement.Api') {
    if (remainder.includes('Migrations/')) return 'api-data'
    if (remainder.startsWith('Data/')) return 'api-data'
    if (remainder.startsWith('Models/') || remainder.startsWith('DTOs/') || remainder.startsWith('Interfaces/')) return 'api-contracts'
    if (remainder.startsWith('Controllers/') || remainder.startsWith('Services/') || remainder.startsWith('Middleware/')) return 'api-services'
    return 'api-project'
  }
  if (project === 'Frontend') {
    if (remainder.endsWith('.spec.ts') || remainder === 'tsconfig.spec.json') return 'angular-tests'
    if (remainder.startsWith('src/app/core/')) return 'angular-core'
    if (remainder.startsWith('src/app/features/auth/')) return 'angular-auth'
    if (remainder.startsWith('src/app/features/employees/') || remainder.startsWith('src/app/shared/')) return 'angular-employees'
    if (remainder.startsWith('src/app/features/dashboard/') || remainder === 'src/styles.scss' || remainder === 'src/app/app.scss') return 'angular-extra'
    return 'angular-project'
  }
  if (project === 'MvcEmployeeManagement') {
    if (remainder.includes('Migrations/')) return 'mvc-data'
    if (remainder.startsWith('Data/') || remainder.startsWith('Models/') || remainder.startsWith('ViewModels/')) return 'mvc-data'
    if (remainder.startsWith('Controllers/')) return 'mvc-controller'
    if (remainder.startsWith('Views/') || remainder.startsWith('wwwroot/')) return 'mvc-views'
    return 'mvc-project'
  }
  throw new Error(`No page assigned for ${relative}`)
}

function purpose(relative) {
  const name = path.posix.basename(relative)
  const directory = path.posix.dirname(relative)
  if (name === 'Program.cs') return 'Configures services, middleware, routes, and development database migration.'
  if (name.endsWith('.csproj')) return 'Defines the .NET target framework and NuGet dependencies.'
  if (name === 'package.json') return 'Defines Angular dependencies and npm commands.'
  if (name === 'angular.json') return 'Configures Angular build, assets, styles, and test targets.'
  if (name.startsWith('tsconfig')) return 'Configures TypeScript compilation for this target.'
  if (name.startsWith('appsettings')) return 'Supplies application settings for this environment; development values are for local practice.'
  if (name.includes('InitialCreate.cs')) return 'Creates the initial database schema and seeded reference data.'
  if (name === 'AppDbContext.cs') return 'Maps entities, relationships, indexes, and seed data to SQL Server.'
  if (name === 'DatabaseInitializer.cs') return 'Applies the API migration and seeds local demo users.'
  if (name.endsWith('Controller.cs')) return `Implements the ${name.replace('Controller.cs', '')} HTTP actions.`
  if (name.endsWith('Service.cs')) return `Implements ${name.replace('Service.cs', '').replace(/^I/, '')} operations.`
  if (name.endsWith('Middleware.cs')) return 'Converts application exceptions into HTTP problem responses.'
  if (directory.includes('/DTOs/')) return 'Defines request validation and API response contracts.'
  if (directory.includes('/ViewModels/')) return 'Defines MVC page inputs, validation, and display state.'
  if (directory.includes('/Models/')) return 'Defines a database or response model used by this application.'
  if (name.endsWith('.spec.ts')) return 'Tests this Angular component.'
  if (name.endsWith('.component.html')) return 'Renders the markup for this Angular screen.'
  if (name.endsWith('.component.ts')) return 'Implements the behavior for this Angular screen or shared control.'
  if (name.endsWith('.guard.ts')) return 'Controls navigation to protected Angular routes.'
  if (name.endsWith('.interceptor.ts')) return 'Adds authentication to requests and handles unauthenticated responses.'
  if (name.endsWith('.model.ts')) return 'Defines typed data exchanged with the API.'
  if (name.endsWith('.cshtml')) return 'Renders this Razor page or reusable partial.'
  if (name.endsWith('.css') || name.endsWith('.scss')) return 'Styles this page or application.'
  if (name === 'environment.ts') return 'Defines the API base URL used by Angular.'
  if (name === 'app.routes.ts') return 'Maps Angular URLs to pages and guards.'
  if (name === 'app.config.ts') return 'Registers Angular router and HTTP services.'
  if (name === 'main.ts') return 'Bootstraps the Angular application.'
  if (name === 'index.html') return 'Hosts the Angular application root.'
  return 'Supplies application code or configuration required by this sample.'
}

function language(relative) {
  if (relative.endsWith('.cs')) return 'csharp'
  if (relative.endsWith('.csproj')) return 'xml'
  if (relative.endsWith('.cshtml') || relative.endsWith('.html')) return 'html'
  if (relative.endsWith('.ts')) return 'typescript'
  if (relative.endsWith('.scss')) return 'scss'
  if (relative.endsWith('.css')) return 'css'
  if (relative.endsWith('.json')) return 'json'
  if (relative.endsWith('.js')) return 'javascript'
  return 'text'
}

function fenceFor(content) {
  const runs = [...content.matchAll(/`+/g)].map(match => match[0].length)
  return '`'.repeat(Math.max(3, ...runs.map(length => length + 1)))
}

const groups = new Map(Object.keys(pages).map(key => [key, []]))
const files = (await walk(root)).sort((a, b) => a.localeCompare(b))
for (const file of files) {
  const relative = `sample/${path.relative(root, file).replaceAll('\\', '/')}`
  const content = (await readFile(file, 'utf8')).replaceAll('\r\n', '\n').trimEnd()
  const fence = fenceFor(content)
  groups.get(pageFor(relative)).push({ relative, content, fence })
}

await mkdir(output, { recursive: true })
for (const [key, title] of Object.entries(pages)) {
  const entries = groups.get(key)
  if (entries.length === 0) throw new Error(`Empty source page: ${key}`)
  const lines = [`# ${title}`, '']
  for (const { relative, content, fence } of entries) {
    lines.push(`## \`${relative}\``, '', purpose(relative), '', `${fence}${language(relative)}`, content, fence, '')
  }
  await writeFile(path.join(output, `${key}.md`), lines.join('\n'), 'utf8')
}
console.log(`Embedded ${files.length} application and project files in ${Object.keys(pages).length} source pages.`)
