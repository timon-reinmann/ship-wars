let player1 = null;
let activeWordCount1 = 0;
let activeWordCount2 = 0;
let messageBox = document.createElement("div");
const chatHubApi = api.replace("api/Game/", "ChatHub");

const SRPChoice = document.querySelectorAll(".SRP-choice");

getUser(gamePlayerId);

const ScissorsRockPaperEnum = {
  Scissors: 0,
  Rock: 1,
  Paper: 2,
};

const scissors = document.querySelector(".scissors");
const rock = document.querySelector(".rock");
const paper = document.querySelector(".paper");
const SRP = document.querySelector(".rock-paper-scissors-container");

let intervalSRP;

let isHuman = true;

async function ScissorsRockPaper() {
  const SRPFindished = await IsSRPIsSet(gamePlayerId);
  if (!SRPFindished) {
    scissors.classList.add("scissors--active");
    rock.classList.add("rock--active");
    paper.classList.add("paper--active");
    SRP.classList.add("SRP--active");
  }
}

function createLoadingScreenForSRP() {
  scissors.classList.remove("scissors--active");
  rock.classList.remove("rock--active");
  paper.classList.remove("paper--active");
  SRP.classList.remove("SRP--active");
  const finishField = document.querySelector(".finish");
  const commit_button = document.querySelector(".commit-button");
  const ring = document.querySelector(".ring");
  const shipSelection = document.querySelector(".ship__selection");
  shipSelection.classList.add("ship__selection--active");
  ring.classList.add("ring--active");
  finishField.classList.add("active-popup");
  commit_button.classList.add("commit-button--active");
}

function deleteLoadingScreenForSRP() {
  const finish = document.querySelector(".finish");
  const commit_button = document.querySelector(".commit-button");
  const ring = document.querySelector(".ring");
  const shipSelection = document.querySelector(".ship__selection");
  shipSelection.classList.remove("ship__selection");
  ring.classList.remove("ring--active");
  finish.classList.remove("active-popup");
  loadShotsFromOpponent();
  countShots();
  remove(commit_button);
}

SRPChoice.forEach((srp) => {
  srp.addEventListener("click", function () {
    localStorage.setItem("srpReload", "true");
    const choice = mapFrontendScissorsRockPaperToBackendEnum(
      srp.dataset.choice
    );

    const API_URL = api + gamePlayerId + "/SaveSRP";
    fetch(API_URL, {
      credentials: "omit",
      headers: {
        "User-Agent":
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
        Accept: "*/*",
        "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
        "Content-Type": "application/json",
        "Sec-Fetch-Dest": "empty",
      },
      body: JSON.stringify(choice),
      method: "PUT",
    }).then((data) => {
      if (data) {
        createLoadingScreenForSRP();
        intervalSRP = setInterval(IsSRPIsSet, 1000);
      }
    });
  });
});
async function IsSRPIsSet() {
  const API_URL = api + gamePlayerId + "/CheckIfSRPIsSet";
  const result = await fetch(API_URL, {
    credentials: "omit",
    headers: {
      "User-Agent":
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
      Accept: "*/*",
      "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
      "Content-Type": "application/json",
      "Sec-Fetch-Dest": "empty",
    },
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.status === 1 || data.status === 2) {
        clearInterval(intervalSRP);
        deleteLoadingScreenForSRP();
        return true;
      }
      if (
        (data.status === 4 || data.status === 3) &&
        localStorage.getItem("srpReload") === "true"
      ) {
        localStorage.setItem("srpReload", "false");
        location.reload();
      }
      return false;
    });
  return result;
}

function mapFrontendScissorsRockPaperToBackendEnum(choice) {
  switch (choice) {
    case "scissors":
      return ScissorsRockPaperEnum.Scissors;
    case "rock":
      return ScissorsRockPaperEnum.Rock;
    case "paper":
      return ScissorsRockPaperEnum.Paper;
    default:
      // Handle ungültige Richtungen oder Fehlerbehandlung hier
      throw new Error("Ungültige Richtung im Frontend: " + frontendDirection);
  }
}

async function getUser(gamePlayerId) {
  const API_URL = api + gamePlayerId + "/GetUser";
  fetch(API_URL, {
    credentials: "omit",
    headers: {
      "User-Agent":
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
      Accept: "*/*",
      "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
      "Content-Type": "application/json",
      "Sec-Fetch-Dest": "empty",
    },
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => {
      player1 = data.user;
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function checkIfMessageIsThere(gameId) {
  const API_URL = api + gameId + "/Message";
  fetch(API_URL, {
    credentials: "omit",
    headers: {
      "User-Agent":
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
      Accept: "*/*",
      "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
      "Content-Type": "application/json",
      "Sec-Fetch-Dest": "empty",
    },
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => {
      let wordCount1 = 0;
      let wordCount2 = 0;
      data.forEach((message) => {
        const timeHHMMSS = message.date.split("T")[1].split(":");
        var li = document.createElement("li");
        document.getElementById("message-list").appendChild(messageBox);

        if (player1 == message.user) {
          activeWordCount1 = 1;
          activeWordCount2 = 0;
          if (wordCount1 >= 1) {
            li.innerHTML = `<p class="li--message">${message.text}</p>`;
          } else {
            li.innerHTML = `<p class="li--time" style="">${timeHHMMSS[0]}:${timeHHMMSS[1]}</p>  &ensp; <p class="li--user">${message.user}:</p> <p class="li--message">${message.text}</p>`;
            messageBox.style.marginTop = "10px";
            messageBox = document.createElement("div");
            document.getElementById("message-list").appendChild(messageBox);
          }
          messageBox.classList.add("li--right");
          messageBox.appendChild(li);
          wordCount1++;
          wordCount2 = 0;
        } else {
          activeWordCount1 = 0;
          activeWordCount2 = 1;
          if (wordCount2 >= 1) {
            li.innerHTML = `<p class="li--message">${message.text}</p>`;
          } else {
            li.innerHTML = `<p class="li--time2">${timeHHMMSS[0]}:${timeHHMMSS[1]}</p>  &ensp; <p class="li--user2">${message.user}:</p> <p class="li--message">${message.text}</p>`;
            messageBox.style.marginTop = "10px";
            messageBox = document.createElement("div");
            document.getElementById("message-list").appendChild(messageBox);
          }
          messageBox.classList.add("li--left");
          messageBox.appendChild(li);
          wordCount2++;
          wordCount1 = 0;
        }
      });
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

const connectionChatHub = new signalR.HubConnectionBuilder()
  .withUrl(chatHubApi)
  .build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

checkIfMessageIsThere(gameId);

connectionChatHub.on("ReceiveMessage", function (user, message, time) {
  if (message.trim() !== "") {
    // split date from yyyy.mm.ddThh:mm:ss to hh:mm:ss
    const timeHHMMSS = time.split("T")[1].split(":");
    let li = document.createElement("li");
    document.getElementById("message-list").appendChild(messageBox);
    if (player1 == user) {
      if (activeWordCount1 >= 1) {
        li.innerHTML = `<p class="li--message">${message}</p>`;
      } else {
        li.innerHTML = `<p class="li--time">${timeHHMMSS[0]}:${timeHHMMSS[1]}</p>  &ensp; <p class="li--user">${user}:</p> <p class="li--message">${message}</p>`;
        messageBox = document.createElement("div");
        messageBox.style.marginTop = "10px";
        document.getElementById("message-list").appendChild(messageBox);
      }
      messageBox.classList.add("li--right");
      messageBox.appendChild(li);
      activeWordCount1 = 1;
      activeWordCount2 = 0;
    } else {
      if (activeWordCount2 >= 1) {
        li.innerHTML = `<p class="li--message">${message}</p>`;
      } else {
        li.innerHTML = `<p class="li--time2">${timeHHMMSS[0]}:${timeHHMMSS[1]}</p>  &ensp; <p class="li--user2">${user}:</p> <p class="li--message">${message}</p>`;
        messageBox.style.marginTop = "10px";
        messageBox = document.createElement("div");
        document.getElementById("message-list").appendChild(messageBox);
      }
      messageBox.classList.add("li--left");
      messageBox.appendChild(li);
      activeWordCount1 = 0;
      activeWordCount2 = 1;
    }
    var messageList = document.getElementById("message-list");
    messageList.scrollTop = messageList.scrollHeight;
  }
});

// create group for the game so that only the players can see the messages
connectionChatHub
  .start()
  .then(function () {
    connectionChatHub.invoke("JoinGroup", gameId).catch(function (err) {
      return console.error(err.toString());
    });
    document.getElementById("sendButton").disabled = false;
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

document
  .getElementById("sendButton")
  .addEventListener("click", function (event) {
    var user = gamePlayerId;
    var message = document.getElementById("messageInput").value;
    connectionChatHub
      .invoke("SendMessage", user, message)
      .catch(function (err) {
        return console.error(err.toString());
      });
    event.preventDefault();
  });

var messageInput = document.getElementById("messageInput");
messageInput.addEventListener("keypress", function (event) {
  if (event.key === "Enter") {
    event.preventDefault();
    document.getElementById("sendButton").click();
    messageInput.value = "";
  }
});

const sendButton = document.getElementById("sendButton");
sendButton.addEventListener("click", function (event) {
  messageInput.value = "";
});

async function commitShips(commit_button) {
  finishField = document.querySelector(".finish");
  const ships = document.getElementsByClassName("ship");
  const ship_positions = Array.from(ships).map((ship) => ({
    ShipType: ship?.dataset.name,
    X: ship?.parentNode.dataset.x,
    Y: ship?.parentNode.dataset.y,
    Direction: mapFrontendDirectionToBackendEnum(ship?.dataset.direction),
    Id: ship?.Id,
  }));

  try {
    await sendShips(ship_positions);
  } catch (error) {
    console.error("failed to send ships", error);
    error_popup(commit_button);
  }
}

commit_button.addEventListener("click", () => {
  let ship_selector = document.querySelector(".ship__selection");
  if (ship_selector.children.length === 0) {
    commitShips(commit_button);
  } else {
    error_popup(commit_button);
  }
});

async function sendShips(Ships) {
  const API_URL = api + gameId + "/SaveShips";
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
    body: JSON.stringify({
      gameId,
      GamePlayerId: gamePlayerId,
      Ships,
      isHuman,
    }),
    method: "POST",
  }).then((response) => {
    if (!response.ok) {
      error_popup(commit_button);
    } else {
      createLoadingScreen();
      intervalid = setInterval(checkIfPlayerReady, 1000);
    }
  });
}

let error_popup__wmark = document.querySelector(".error-popup__xmark-icon");
error_popup__wmark.addEventListener("click", () => {
  let error_popup__screen_blocker = document.querySelector(
    ".error-popup__screen-blocker"
  );
  let error_popup = document.querySelector(".error-popup");
  error_popup.classList.remove("error-popup--active");
  error_popup__screen_blocker.classList.remove(
    "error-popup__screen-blocker--active"
  );
  commit_button.classList.remove("commit-button--active");
});

function error_popup(commit_button) {
  const error_popup__screen_blocker = document.querySelector(
    ".error-popup__screen-blocker"
  );
  const error_popup = document.querySelector(".error-popup");
  error_popup.classList.add("error-popup--active");
  error_popup__screen_blocker.classList.add(
    "error-popup__screen-blocker--active"
  );
  commit_button.classList.add("commit-button--active");
}
