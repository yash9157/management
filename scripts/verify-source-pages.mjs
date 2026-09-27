import { readdir, readFile } from 'node:fs/promises'
import path from 'node:path'

const sourceRoot = path.resolve('sample')
const docsRoot = path.resolve('docs/source')
const ignoredDirectories = new Set(['bin', 'obj', 'node_modules', 'dist', '.angular', '.vs', '.git'])
const codeExtensions = new Set(['.cs', '.cshtml', '.ts', '.html', '.scss', '.css', '.json', '.sln', '.csproj', '.http', '.js', '.md'])
const extraNames = new Set(['.gitignore', '.editorconfig', '.prettierrc'])

async function sourceFiles(directory) {
  const files = []
  for (const entry of await readdir(directory, { withFileTypes: true })) {
    const fullPath = path.join(directory, entry.name)
    if (entry.isDirectory()) {
      if (!ignoredDirectories.has(entry.name)) files.push(...await sourceFiles(fullPath))
      continue
    }
    if (!entry.isFile() || entry.name.endsWith('.csproj.user')) continue
    if (fullPath.includes(`${path.sep}wwwroot${path.sep}lib${path.sep}`) && !entry.name.endsWith('.js')) continue
    if (codeExtensions.has(path.extname(entry.name)) || extraNames.has(entry.name)) files.push(fullPath)
  }
  return files
}

const embedded = new Map()
for (const page of await readdir(docsRoot)) {
  if (!page.endsWith('.md') || page === 'index.md') continue
  const lines = (await readFile(path.join(docsRoot, page), 'utf8')).split('\n')
  for (let index = 0; index < lines.length; index++) {
    const match = lines[index].match(/^## `([^`]+)`$/)
    if (!match) continue
    const relative = match[1]
    if (embedded.has(relative)) throw new Error(`Duplicate code block: ${relative}`)
    while (++index < lines.length && !/^`{3,}\w*$/.test(lines[index])) {}
    if (index === lines.length) throw new Error(`Missing opening fence: ${relative}`)
    const fence = lines[index].match(/^`+/)[0]
    const body = []
    while (++index < lines.length && lines[index] !== fence) body.push(lines[index])
    if (index === lines.length) throw new Error(`Missing closing fence: ${relative}`)
    embedded.set(relative, body.join('\n'))
  }
}

const source = await sourceFiles(sourceRoot)
const expected = new Set()
for (const file of source) {
  const relative = `sample/${path.relative(sourceRoot, file).replaceAll('\\', '/')}`
  expected.add(relative)
  const actual = embedded.get(relative)
  if (actual === undefined) throw new Error(`Missing from docs: ${relative}`)
  const content = (await readFile(file, 'utf8')).replaceAll('\r\n', '\n').trimEnd()
  if (actual !== content) throw new Error(`Stale or incomplete code block: ${relative}`)
}
for (const relative of embedded.keys()) {
  if (!expected.has(relative)) throw new Error(`Docs contain a removed source file: ${relative}`)
}
console.log(`Verified ${source.length} complete source files in ${await readdir(docsRoot).then(items => items.filter(item => item.endsWith('.md') && item !== 'index.md').length)} documentation pages.`)
