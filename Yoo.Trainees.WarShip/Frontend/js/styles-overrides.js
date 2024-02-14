const hamburger = document.querySelector(".hamburger");
const navBarListUl = document.querySelector(".navbar__list ul");
const navBarList = document.querySelector(".navbar__list");

hamburger.addEventListener("click", (e) => {
  e.preventDefault();
  const oldWidth = getComputedStyle(navBarList).width;
  navBarListUl.setAttribute("style", "display: flex !important;");
  navBarList.setAttribute("style", "width: 60vw !important;");
  hamburger.setAttribute("style", "display: none !important;");
  setTimeout(() => {
    navBarListUl.setAttribute("style", "display: none !important;");
    navBarList.setAttribute("style", `width: ${oldWidth} !important;`);
    hamburger.setAttribute("style", "display: block !important;");
  }, 5000);
});
