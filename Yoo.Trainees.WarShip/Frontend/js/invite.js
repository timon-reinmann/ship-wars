const copyText = document.querySelector(".copy__text");
const submit_button = document.getElementById("lobbyinput");
const send_email = document.getElementById("email");
const wrapper = document.querySelector(".wrapper");
const loginLink = document.querySelector(".login-link");
const registerLink = document.querySelector(".register-link");
const btnPopup = document.querySelector(".btnLogin-popup");
const iconClose = document.querySelector(".icon-close");
const joinGame = document.querySelector(".join__button");
const input = copyText.querySelector("input.text");

let link = null;
let lobbyName = null;

copyText.querySelector("button").addEventListener("click", () => {
  joinGame.classList.add("active");
  input.select();
  document.execCommand("copy");
  copyText.classList.add("active");
  window.getSelection().removeAllRanges();
  setTimeout(function () {
    copyText.classList.remove("active");
  }, 2500);
});

registerLink.addEventListener("click", () => {
  joinGame.classList.remove("active");
  wrapper.classList.add("active");
});

loginLink.addEventListener("click", () => {
  wrapper.classList.remove("active");
  joinGame.classList.remove("active");
});

btnPopup.addEventListener("click", () => {
  wrapper.classList.add("active-popup");
});

iconClose.addEventListener("click", () => {
  joinGame.classList.remove("active");
  wrapper.classList.remove("active-popup");
});

submit_button.addEventListener("click", async function () {
  const createGame = document.querySelector(".submit__lobbyname");
  createGame.classList.add("active");
  setTimeout(() => {
    createGame.classList.remove("active");
  }, 2500);

  lobbyName = JSON.stringify(document.getElementById("lobbyname").value);
  ("use strict");
  await fetch(api, {
    credentials: "omit",
    headers: {
      "User-Agent":
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
      Accept: "*/*",
      "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
      "Content-Type": "application/json",
      "Sec-Fetch-Dest": "empty",
    },
    body: lobbyName,
    method: "POST",
  })
    .then((response) => response.json())
    .then((data) => {
      joinGame.href = data.player1;
      link = data.player2;
      document.getElementById("linkoutput").value = link;
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
});

send_email.addEventListener("click", async () => {
  const email = document.getElementById("email-input").value;
  joinGame.classList.add("active");
  let daten = {
    LobbyName: lobbyName,
    email: email,
    link: link,
  };
  daten = JSON.stringify(daten);
  ("use strict");

  const API_URL_Email = api + "Email";
  fetch(API_URL_Email, {
    credentials: "omit",
    headers: {
      "User-Agent":
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
      Accept: "*/*",
      "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
      "Content-Type": "application/json",
      "Sec-Fetch-Dest": "empty",
    },
    body: daten,
    method: "POST",
  })
    .then((response) => response.json())
    .then((data) => {})
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
});

document.addEventListener("DOMContentLoaded", function () {
  const button = document.getElementById("moving-button");
  moveButton();

  function moveButton() {
    const windowWidth = window.innerWidth;
    const buttonWidth = button.offsetWidth;
    const maxLeft = windowWidth - buttonWidth;

    // Set the button's position to the right edge of the window
    button.style.left = maxLeft + "px";

    // Animate the button's movement from right to left
    setTimeout(() => {
      button.style.left = "0";
      // After the animation, move it back to the right and continue the loop
      setTimeout(moveButton, 2000); // 2000ms (2 seconds) for the transition
    }, 2000); // 2000ms (2 seconds) delay before starting the animation
  }
});
