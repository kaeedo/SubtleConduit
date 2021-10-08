module.exports = {
  purge: ['./index.html', './build/**/*.js'],
  mode: "jit",
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {
      colors:{
        'conduit-green':'#5CB85C'
      },
      textColors:{
        'conduit-green':'#5CB85C'
      }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [require('tailwindcss-textshadow')],
};
