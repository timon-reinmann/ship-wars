const alertBox = document.querySelector(".alert__box");
const overlay = document.querySelector(".overlay");

if (screen.orientation.type === "portrait-primary") {
  changeOrientationAlert();
}

screen.orientation.addEventListener("change", (e) => {
  if (screen.orientation.type === "portrait-primary") {
    changeOrientationAlert();
  } else {
    alertBox.classList.remove("alert__box--show");
    overlay.classList.remove("overlay--show");
  }
});

function changeOrientationAlert() {
  overlay.classList.add("overlay--show");
  alertBox.classList.add("alert__box--show");
  alertBox.innerHTML = `<p class="alert__text">Please rotate your device to landscape mode</p>`;
}
