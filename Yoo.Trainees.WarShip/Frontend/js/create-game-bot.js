const submit_button = document.getElementById("lobbyinput");
const joinGame = document.querySelector(".join__button");

let link = null;
let lobbyName = null;

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
