/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{html,razor,cshtml}",
    "./wwwroot/**/*.html"
  ],
  theme: {
    extend: {
      colors: {
        'ut-yellow': '#ffcc00',
        'ut-dark': '#222222',
        'ut-gray': '#f4f4f4',
        'primary': {
          50: '#fffbeb',
          100: '#fef3c7',
          200: '#fde68a',
          300: '#fcd34d',
          400: '#fbbf24',
          500: '#ffcc00',
          600: '#e6b800',
          700: '#b38f00',
          800: '#806600',
          900: '#4d3d00',
        },
        'ut-gold': '#e8a000',
        'ut-bg': '#1a1a1a',
        'ut-muted': '#8a8780',
        'ut-success': '#1a7a30',
        'ut-danger': '#d94040',
        'ut-warning': '#92400e',
        'ut-info': '#1a6b8a',
      },
    },
  },
  plugins: [],
}
