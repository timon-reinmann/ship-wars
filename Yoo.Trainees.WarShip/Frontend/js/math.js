document.addEventListener("DOMContentLoaded", () => {
  e();
});

window.addEventListener("resize", () => {
  e();
});

function e() {
  const navBar = document.querySelector(".navbar__list");

  let h = navBar.offsetHeight;
  let alpha = 30;

  let alphaInRadians = alpha * (Math.PI / 180);

  // Berechnung von h * tan(alpha)
  let leftPosition = h * Math.tan(alphaInRadians);

  navBar.style.right = `calc(0% - ${leftPosition}px)`;
}
