/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "./Components/**/*.{razor,cs}",
    "./Pages/**/*.razor"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: ['-apple-system', 'BlinkMacSystemFont', 'SF Pro Text', 'SF Pro Display',
               'Helvetica Neue', 'Helvetica', 'Arial', 'PingFang SC', 'Hiragino Sans GB',
               'Microsoft YaHei', 'sans-serif']
      },
      colors: {
        brand: {
          50:  '#f5f7ff',
          100: '#e8ecff',
          200: '#c9d2ff',
          300: '#a4b1ff',
          400: '#7a8bff',
          500: '#5a6cf5',
          600: '#4553d9',
          700: '#3740ad',
          800: '#2a3186',
          900: '#1d2260'
        }
      },
      boxShadow: {
        soft: '0 4px 16px -2px rgba(15, 23, 42, 0.08), 0 2px 6px -2px rgba(15, 23, 42, 0.06)',
        pop: '0 12px 32px -8px rgba(15, 23, 42, 0.18), 0 4px 10px -4px rgba(15, 23, 42, 0.08)'
      },
      borderRadius: {
        '2xl': '1.25rem',
        '3xl': '1.75rem'
      }
    }
  },
  plugins: []
};
