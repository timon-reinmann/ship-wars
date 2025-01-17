const copyText = document.querySelector(".copy__text");
const submit_button = document.getElementById("lobbyinput");
const send_email = document.getElementById("email");
const wrapper = document.querySelector(".wrapper");
const loginLink = document.querySelector(".login-link");
const registerLink = document.querySelector(".register-link");
const btnPopup = document.querySelector(".btnLogin-popup");
const iconClose = document.querySelector(".icon-close");

let link = null;
let lobbyName = null;


copyText.querySelector("button").addEventListener("click", () => {
  let input = copyText.querySelector("input.text");
  input.select();
  document.execCommand("copy");
  copyText.classList.add("active");
  window.getSelection().removeAllRanges();
  setTimeout(function () {
    copyText.classList.remove("active");
  }, 2500);
});

registerLink.addEventListener("click", () => {
  wrapper.classList.add("active");
});

loginLink.addEventListener("click", () => {
  wrapper.classList.remove("active");
});

btnPopup.addEventListener("click", () => {
  wrapper.classList.add("active-popup");
});

iconClose.addEventListener("click", () => {
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
  const API_URL = "https://localhost:7118/api/Game";
  await fetch(API_URL, {
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
    .then((response) => response.text())
    .then((data) => {
      console.log("Daten:", data);
      link =
        "http://127.0.0.1:5500/Frontend/html/game-pvp.html?playerid=" + data;
      document.getElementById("linkoutput").value = link;
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
});

send_email.addEventListener("click", async () => {
  let email = document.getElementById("email-input").value;

  let daten = {
    gameName: lobbyName,
    email: email,
    link: link,
  };
  daten = JSON.stringify(daten);
  ("use strict");

  const API_URL_Email = "https://localhost:7118/api/Game/Email";
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
    .then((data) => {
      console.log("Daten");
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
});
