const submit_button = document.getElementById("lobbyinput");
const joinGame = document.querySelector(".join__button");
const hard_button = document.querySelector(".hard-mode");
const easy_button = document.querySelector(".easy-mode");

let hard_game = false;
let easy_game = false;

let link = null;
let lobbyName = null;
let isFetchEnable = false;

const isBotGame = true;

hard_button.addEventListener("click", function () {
  hard_button.classList.add("hard-mode--active");
  easy_button.classList.remove("easy-mode--active");
  hard_game = true;
  easy_game = false;
  isFetchEnable = true;
});

easy_button.addEventListener("click", function () {
  easy_button.classList.add("easy-mode--active");
  hard_button.classList.remove("hard-mode--active");
  easy_game = true;
  hard_game = false;
  isFetchEnable = true;
});

submit_button.addEventListener("click", async function () {
  lobbyName = document.getElementById("lobbyname").value;

  if (!isFetchEnable) {
    error_popup();
  } else {
    joinGame.classList.add("active");
    const createGame = document.querySelector(".submit__lobbyname");
    createGame.classList.add("active");
    setTimeout(() => {
      createGame.classList.remove("active");
    }, 2500);
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
      body: JSON.stringify({
        Name: lobbyName,
        Bot: isBotGame,
        EasyGame: easy_game,
      }),
      method: "POST",
    })
      .then((response) => response.json())
      .then((data) => {
        joinGame.href = data.player1 + "&gameMode=" + hard_game;
        console.log(data.player1);
      })
      .catch((error) => {
        console.error("Es gab einen Fehler bei der Anfrage:", error);
      });
  }
});

function error_popup() {
  const error_popup__screen_blocker = document.querySelector(
    ".error-popup__screen-blocker"
  );
  const error_popup__text = document.querySelector(".error-popup__text");
  const error_popup = document.querySelector(".error-popup");
  const error_popup_xmark_icon = document.querySelector(
    ".error-popup__xmark-icon"
  );
  error_popup.classList.add("error-popup--active");
  error_popup__text.classList.add(".error-popup__text--active");
  error_popup_xmark_icon.classList.add("..error-popup__xmark-icon--active");
  error_popup__screen_blocker.classList.add(
    "error-popup__screen-blocker--active"
  );
}
