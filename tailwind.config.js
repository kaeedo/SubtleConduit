module.exports = {
  purge: ['./index.html', './build/**/*.js'],
  mode: "jit",
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {
      colors:{
        'conduit-green':'#5CB85C',
        'conduit-green-500':'#449d44'
      },
      textColors:{
        'conduit-green':'#5CB85C',
        'conduit-green-500':'#449d44'
      }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [require('tailwindcss-textshadow')],
};
