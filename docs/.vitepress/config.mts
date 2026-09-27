import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'Employee Management Lab',
  description: 'Source-backed ASP.NET Core, EF Core, SQL Server, Angular, and MVC interview guide',
  cleanUrls: true,
  lastUpdated: true,
  themeConfig: {
    siteTitle: 'Employee Management Lab',
    nav: [
      { text: 'All files', link: '/source/' },
      { text: 'Setup', link: '/start' },
      { text: 'Download', link: '/sample-source.zip' }
    ],
    sidebar: {
      '/source/': [
        {
          text: 'EmployeeManagement.Api',
          link: '/source/api-project',
          collapsed: true,
          items: [
            { text: 'Project and startup', link: '/source/api-project' },
            { text: 'Models and DTOs', link: '/source/api-contracts' },
            { text: 'EF Core and migration', link: '/source/api-data' },
            { text: 'Controllers and services', link: '/source/api-services' },
            { text: 'Migration metadata', link: '/source/api-generated' }
          ]
        },
        {
          text: 'Frontend',
          link: '/source/angular-project',
          collapsed: true,
          items: [
            { text: 'Project and bootstrap', link: '/source/angular-project' },
            { text: 'Dependency lockfile', link: '/source/angular-lockfile' },
            { text: 'Models and services', link: '/source/angular-core' },
            { text: 'Login and registration', link: '/source/angular-auth' },
            { text: 'Employee screens', link: '/source/angular-employees' },
            { text: 'Other UI files', link: '/source/angular-extra' },
            { text: 'Tests', link: '/source/angular-tests' }
          ]
        },
        {
          text: 'MvcEmployeeManagement',
          link: '/source/mvc-project',
          collapsed: true,
          items: [
            { text: 'Project and startup', link: '/source/mvc-project' },
            { text: 'Models and migration', link: '/source/mvc-data' },
            { text: 'Controllers', link: '/source/mvc-controller' },
            { text: 'Razor views and styles', link: '/source/mvc-views' },
            { text: 'Bundled JavaScript', link: '/source/mvc-vendor' },
            { text: 'Migration metadata', link: '/source/mvc-generated' }
          ]
        }
      ]
    },
    search: { provider: 'local' },
    outline: { level: [2, 3] },
    editLink: undefined,
    footer: { message: 'Complete sample source, file by file', copyright: 'Employee Management interview practice' }
  }
})
