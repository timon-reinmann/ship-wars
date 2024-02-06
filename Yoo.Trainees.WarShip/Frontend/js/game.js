const stateLabel = document.getElementById("socketState");

let player1 = null;
let activeWordCount1 = 0;
let activeWordCount2 = 0;
let messageBox = document.createElement("div");
const chatHubApi = api.replace("api/Game/", "ChatHub");
const gameHubApi = api.replace("api/Game/", "GameHub");

// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
const gameId = urlParams.get("gameId");
const gamePlayerId = urlParams.get("gamePlayerId");
const SRPChoice = document.querySelectorAll(".SRP-choice");
let lastContainer = null;

// Informationbox
const dialogCloseButton = document.querySelector(".dialog--close");
const informationDialog = document.querySelector(".information__dialog");
const informationButton = document.querySelector(".information__button");

const connectionGameHub = new signalR.HubConnectionBuilder()
  .withUrl(gameHubApi)
  .build();

connectionGameHub.on("LoadShotsFromOpponent", function (shots) {
  shots.forEach((shot) => {
    const X = shot.x;
    const Y = shot.y;
    const opponentFields = document.getElementById("game__board");
    const opponentField = opponentFields.querySelector(
      `[data-x="${X}"][data-y="${Y}"]`
    );
    opponentField.classList.add("field--hit");
  });
});

connectionGameHub.on("CountShots", function (shots, nextPlayer, gameState) {
  showCountShots(shots, nextPlayer, gameState);
});

connectionGameHub
  .start()
  .then(function () {
    connectionGameHub.invoke("JoinGroup", gameId).catch(function (err) {
      return console.error(err.toString());
    });
    connectionGameHub.invoke("EgoGroup", gamePlayerId).catch(function (err) {
      return console.error(err.toString());
    });
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

connectionGameHub
  .invoke("LoadShotsFromOpponent", gamePlayerId)
  .catch(function (err) {
    return console.error(err.toString());
  });

Promise.all([CheckIfBoardSet(gamePlayerId), loadFiredShots(gamePlayerId)]);
loadHitShips(gamePlayerId);

const muteButton = document.querySelector(".mute__button");
let mute = false;

getUser(gamePlayerId);

let boardState = new Array(10).fill(null).map(() => new Array(10).fill(0));
let originField = null;
let toggle = false;
let myBoard = document.getElementById("game__board");
let gameOpponent = document.getElementById("opponent__board");

let finishField = null;
const commit_button = document.querySelector(".commit-button");

const DirectionEnum = {
  HORIZONTAL: 0,
  VERTICAL: 1,
};
const ScissorsRockPaperEnum = {
  Scissors: 0,
  Rock: 1,
  Paper: 2,
};
const GameStateEnum = {
  Won: 1,
  Lost: 2,
};

const sound = new Audio("../sound/pewpew.mp3");

createBoard(myBoard, true);
createBoard(gameOpponent, false);

let zIndexChange = 1;
let currentField = null;

let intervalid;
let intervalSRP;
let hoverTimer = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".ownField");
const shipSelection = document.querySelector(".ship__selection");
const opponentFields = document.querySelectorAll(".opponentField");

const scissors = document.querySelector(".scissors");
const rock = document.querySelector(".rock");
const paper = document.querySelector(".paper");
const SRP = document.querySelector(".rock-paper-scissors-container");

localStorage.setItem("srpReload", "false");

informationButton.addEventListener("click", () => {
  informationDialog.classList.add("animation__dialog--in");
  informationDialog.showModal();
});

informationDialog.addEventListener("animationend", () => {
  informationDialog.classList.remove("animation__dialog--in");
});

dialogCloseButton.addEventListener("click", () => {
  informationDialog.classList.add("animation__dialog--out");
  setTimeout(() => {
    informationDialog.close();
    informationDialog.classList.remove("animation__dialog--out");
  }, 1000);
});

draggables.forEach((draggable) => {
  draggable.addEventListener("mouseover", (e) => {
    let currentShip = draggable.parentNode;
    const currentX = parseInt(currentShip.getAttribute("data-x"));
    const currentY = parseInt(currentShip.getAttribute("data-y"));
    const isValid = isDirectionChangeAllowed(
      draggable,
      currentX,
      currentY,
      parseInt(draggable.getAttribute("data-size"))
    );
    if (isValid && !draggable.classList.contains(".dragging")) {
      draggable.style.setProperty("--opacityBefore", 1);
      hoverTimer = setTimeout(() => {
        draggable.style.setProperty("--opacityAfter", 1);
      }, 3000);
    }
  });
  draggable.addEventListener("mouseout", (e) => {
    draggable.style.setProperty("--opacityBefore", 0);
    draggable.style.setProperty("--opacityAfter", 0);
    clearTimeout(hoverTimer);
  });
  draggable.addEventListener("click", (e) => {
    click(draggable);
  });

  draggable.addEventListener("dragstart", (e) => {
    dragstart(draggable, e);
  });

  draggable.addEventListener("dragend", (e) => {
    dragend(draggable, e);
  });

  draggable.addEventListener("drag", (e) => {
    const x = e.clientX;
    const y = e.clientY;
    const container = document.elementFromPoint(x, y);
    const isPlacementValid = dragover(container, e);
    if (isPlacementValid) {
      lastContainer = container;
    }
    if (!originField.classList.contains("ownField")) {
      dragMove(draggable, x, y);
    }
  });

  draggable.addEventListener("touchstart", (e) => {
    dragstart(draggable, e);
  });

  draggable.addEventListener("touchmove", (e) => {
    const touch = e.touches[0];
    const x = touch.clientX;
    const y = touch.clientY;
    const container = document.elementFromPoint(x, y);
    const isPlacementValid = dragover(container, e);
    if (isPlacementValid) {
      lastContainer = container;
    }

    if (!originField.classList.contains("ownField")) {
      dragMove(draggable, x, y);
    }
  });

  draggable.addEventListener("touchend", (e) => {
    dragend(draggable, e);
    const parentElement = draggable.parentNode;
    if (parentElement === originField) {
      click(draggable);
    }
  });
});

function click(draggable) {
  let currentShip = draggable.parentNode;
  const currentX = parseInt(currentShip.getAttribute("data-x"));
  const currentY = parseInt(currentShip.getAttribute("data-y"));
  const shipSize = parseInt(currentShip.firstChild.getAttribute("data-size"));
  const isValid = isDirectionChangeAllowed(
    draggable,
    currentX,
    currentY,
    parseInt(draggable.getAttribute("data-size"))
  );
  if (isValid) {
    draggable.setAttribute("data-direction", "vertical");
    toggle = draggable.classList.toggle("vertical");
    changeHitBoxOnClick(toggle, currentX, currentY, shipSize, 0);
    changeHitBoxOnClick(!toggle, currentX, currentY, shipSize, shipSize);
  }
}

function dragstart(draggable, e) {
  let img = new Image();
  img.src = "../img/invisivble_ship.png";
  try {
    e.dataTransfer.setDragImage(img, 0, 0);
  } catch (error) {
    // If datatransfer is not supported, do nothing because its the phone version
  }
  originField = draggable.parentNode;
  draggable.classList.add("dragging");
  deleteShipHitBox(draggable.parentNode);
}

function dragMove(draggable, x, y) {
  const fakeShip = document.querySelector(".ship--fake");
  const imgName = draggable.getAttribute("data-name");
  const imgURL = "../img/" + imgName + ".png";
  const computedStyle = window.getComputedStyle(draggable);
  const gemeinsamerTeiler = draggable.offsetWidth % 52;

  fakeShip.classList.add("ship--active");

  fakeShip.style.backgroundImage = `url(${imgURL})`;
  fakeShip.style.width = draggable.offsetWidth + "px";
  fakeShip.style.height = draggable.offsetHeight + "px";
  fakeShip.style.backgroundSize = computedStyle.backgroundSize;
  fakeShip.style.left =
    x - draggable.offsetWidth / 2 - 52 * (gemeinsamerTeiler - 1) + "px";
  fakeShip.style.top = y - draggable.offsetHeight / 2 + "px";
}

function dragend(draggable, e) {
  let isPlacementValid = true;
  if (
    dragover(lastContainer, e) &&
    !lastContainer.classList.contains("opponentField")
  ) {
    lastContainer.appendChild(draggable);
  } else {
    isPlacementValid = false;
    originField.appendChild(draggable);
  }
  const fakeShip = document.querySelector(".ship--fake");
  fakeShip.classList.remove("ship--fake--active");
  draggable.classList.remove("dragging");

  draggable.setAttribute("style", "position: static;");
  currentField.style.zIndex = zIndexChange + 1;
  const currentX = parseInt(
    isPlacementValid
      ? currentField.getAttribute("data-x")
      : originField.getAttribute("data-x")
  );
  const currentY = parseInt(
    isPlacementValid
      ? currentField.getAttribute("data-y")
      : originField.getAttribute("data-y")
  );
  const shipSize = parseInt(draggable.getAttribute("data-size"));
  for (let i = -1; i <= shipSize; i++) {
    for (let j = -1; j < 2; j++) {
      const field =
        draggable.getAttribute("data-direction") !== "vertical"
          ? document.querySelector(
              `[data-x="${currentX + i}"][data-y="${currentY + j}"]`
            )
          : document.querySelector(
              `[data-x="${currentX + j}"][data-y="${currentY + i}"]`
            );
      if (field) {
        field.setAttribute(
          "data-ships",
          parseInt(field.getAttribute("data-ships")) + 1
        );
      }
    }
  }
  zIndexChange++;
}

function dragover(container, e) {
  e.preventDefault();
  if (container.firstChild === null) {
    container.style.zIndex = 0;
  }
  const draggable = document.querySelector(".dragging");
  if (!draggable) return false;

  const shipSize = parseInt(draggable.getAttribute("data-size"));
  const currentX = parseInt(container.getAttribute("data-x"));
  const currentY = parseInt(container.getAttribute("data-y"));
  let shipCheck = 0;

  const checkField = document.querySelector(
    `[data-x="${currentX}"][data-y="${currentY}"]`
  );
  if (checkField) {
    const dataCheck = parseInt(checkField.getAttribute("data-size"));
    if (!isNaN(dataCheck)) {
      shipCheck = dataCheck;
    }
  }
  // Check if there is enough space for the ship
  let isPlacementValid = true;

  for (let i = 0; i < shipSize; i++) {
    const freeField =
      draggable.dataset.direction !== "vertical"
        ? document.querySelector(
            `[data-x="${currentX + i}"][data-y="${currentY}"]`
          )
        : document.querySelector(
            `[data-x="${currentX}"][data-y="${currentY + i}"]`
          );

    if (
      !freeField ||
      freeField.getAttribute("data-ships") > 0 ||
      freeField.classList.contains("opponentField")
    ) {
      // There is a obstacle or the field isn't on the game board anymore
      isPlacementValid = false;
      break;
    }
    if (shipCheck > 0) {
      isPlacementValid = false;
    }
  }
  if (isPlacementValid) {
    // Place the ship and set data-size for all fields
    container.appendChild(draggable);
    currentField = container; // update the currnt Field value
    draggable.classList.remove("invalid");
  } else {
    // Ship can't be placed
    draggable.classList.add("invalid");
  }
  return isPlacementValid;
}
containers.forEach((container) => {
  container.addEventListener("dragover", (e) => {
    dragover(container, e);
    const isPlacementValid = dragover(container, e);
    if (isPlacementValid) {
      lastContainer = container;
    }
  });

  // Komischer Weise geht das auch mit drag anstatt drop
  container.addEventListener("drop", (e) => {
    e.preventDefault();
  });
});

opponentFields.forEach((opponentField) => {
  opponentField.addEventListener("click", async (e) => {
    const isReadyToShoot = await checkReadyToShoot(gamePlayerId);
    if (!isReadyToShoot) {
      return;
    }
    const currentX = parseInt(opponentField.getAttribute("data-x"));
    const currentY = parseInt(opponentField.getAttribute("data-y"));
    const API_URL = api + gamePlayerId + "/SaveShot";
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
      body: JSON.stringify({ X: currentX, Y: currentY }),
      method: "POST",
    })
      .then((response) => response.json())
      .then((data) => {
        sound.play();
        const cursor = document.querySelector(".cursor");
        cursor.classList.add("recoil-animation");
        setTimeout(() => {
          cursor.classList.remove("recoil-animation");
        }, 200);
        if (data.hit === 1 || data.hit === 0) {
          opponentField.classList.add("field--hit");
          connectionGameHub
            .invoke("CountShots", gamePlayerId)
            .catch(function (err) {
              return console.error(err.toString());
            });
          connectionGameHub
            .invoke("LoadShotsFromOpponent", gamePlayerId)
            .catch(function (err) {
              return console.error(err.toString());
            });
        }
        if (data.hit === 1) {
          opponentField.classList.add("field--hit--ship");
          showExplosionAnimation(opponentField);
        }
      });
  });
});

shipSelection.addEventListener("dragover", (e) => {
  e.preventDefault();
  const draggable = document.querySelector(".dragging");
  draggable.classList.remove("invalid");
  draggable.classList.remove("vertical");
  shipSelection.appendChild(draggable);
  currentField = null;
});

function mapFrontendDirectionToBackendEnum(frontendDirection) {
  switch (frontendDirection) {
    case "horizontal":
      return DirectionEnum.HORIZONTAL;
    case "vertical":
      return DirectionEnum.VERTICAL;
    default:
      // Handle ungültige Richtungen oder Fehlerbehandlung hier
      throw new Error("Ungültige Richtung im Frontend: " + frontendDirection);
  }
}

muteButton.addEventListener("mouseover", () => {
  muteButton.classList.add("fa-bounce");
});
muteButton.addEventListener("mouseout", () => {
  muteButton.classList.remove("fa-bounce");
});
muteButton.addEventListener("click", () => {
  mute = !mute;
  if (mute) {
    muteButton.children[0].classList.add("fa-volume-xmark");
    muteButton.children[0].classList.remove("fa-volume-high");
    sound.volume = 0;
  } else {
    muteButton.children[0].classList.remove("fa-volume-xmark");
    muteButton.children[0].classList.add("fa-volume-high");
    sound.volume = 1;
  }
});

function deleteShipHitBox(container) {
  if (originField) {
    const oldX = parseInt(originField.dataset.x);
    const oldY = parseInt(originField.dataset.y);
    const oldShipSize = parseInt(originField.firstChild?.dataset.size);
    const startingPoint = -1; // -1, because the ship also need space for the fields around him
    const shipWidth = 2; // 2 because the ship is allways the same width (horizontal an vertical)
    const isVertical = container.firstChild?.dataset.direction === "vertical";
    for (let i = startingPoint; i <= oldShipSize; i++) {
      for (let j = startingPoint; j < shipWidth; j++) {
        const oldField = document.querySelector(
          `[data-x="${oldX + (!isVertical ? i : j)}"][data-y="${
            oldY + (!isVertical ? j : i)
          }"]`
        );
        if (oldField) {
          const currentShips = parseInt(oldField.dataset.ships, 10) || 0;
          oldField.dataset.ships = Math.max(0, currentShips - 1);
        }
      }
    }
  }
}

function changeHitBoxOnClick(
  toggleOnClick,
  currentX,
  currentY,
  shipSize,
  fieldSize
) {
  for (let i = -1; i <= shipSize; i++) {
    for (let j = -1; j < 2; j++) {
      const field = document.querySelector(
        `[data-x="${currentX + (toggleOnClick ? i : j)}"][data-y="${
          currentY + (toggleOnClick ? j : i)
        }"]`
      );
      if (field) {
        const currentShips = parseInt(field.dataset.ships, 10) || 0;
        field.dataset.ships = Math.max(
          0,
          currentShips + (fieldSize > 0 ? 1 : -1)
        );
        if (fieldSize > 0 && field.firstChild) {
          field.firstChild.dataset.direction = toggleOnClick
            ? "horizontal"
            : "vertical";
        }
      }
    }
  }
}

function createBoard(gameBoard, isMyBoard) {
  let countingFields = 0;
  const maxBoardLength = 10;
  for (let y = 0; y < maxBoardLength; y++) {
    for (let x = 0; x < maxBoardLength; x++) {
      const div = document.createElement("div");
      div.classList.add("field", `b${countingFields}`);
      div.dataset.x = x;
      div.dataset.y = y;
      if (isMyBoard) {
        div.classList.add("ownField");
        div.id = `box${countingFields}`;
        div.dataset.ships = 0;
      }
      if (!isMyBoard) {
        div.classList.add("opponentField");
      }
      gameBoard.appendChild(div);
      countingFields += 1;
    }
  }
}

function isDirectionChangeAllowed(draggable, currentX, currentY, shipSize) {
  const nextPossibleField = 2; // Because all ships need 1 field apart from each other so we check on the field 2 0, 1 ,2 <-- 2 is the next possible field
  const isVertical = draggable.dataset.direction === "vertical";
  const tinyShip = shipSize === 2 ? 1 : 0; // if we compare i < shipSize we see that its false because i = 2 and shipSize = 2 so we need to treat this case differently
  // So we need to check earlier if there is a Ship but because our ship is also in that field we need to check if there are 2 ships not 1

  for (let i = nextPossibleField - tinyShip; i < shipSize; i++) {
    const x = !isVertical ? currentX : currentX + i;
    const y = !isVertical ? currentY + i : currentY;
    const futureField = document.querySelector(
      `[data-x="${x}"][data-y="${y}"]`
    );
    if (futureField.dataset.ships > 0 + tinyShip) {
      return false; // Is not valid
    }
  }
  return true; // Is valid
}
// ...
commit_button.addEventListener("click", (e) => {
  e.preventDefault();
  let ship_selector = document.querySelector(".ship__selection");
  if (ship_selector.children.length === 0) {
    commitShips(commit_button);
  } else {
    error_popup(commit_button);
  }
});

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
    body: JSON.stringify({ gameId, GamePlayerId: gamePlayerId, Ships }),
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

function createLoadingScreen() {
  const finishField = document.querySelector(".finish");
  const commit_button = document.querySelector(".commit-button");
  const ring = document.querySelector(".ring");
  const shipSelection = document.querySelector(".ship__selection");
  shipSelection.classList.add("ship__selection--active");
  ring.classList.add("ring--active");
  finishField.classList.add("active-popup");
  commit_button.classList.add("commit-button--active");
}

function checkIfPlayerReady() {
  const API_URL = api + gameId + "/Ready";
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
    .then((data) => {
      if (data.ok) {
        clearInterval(intervalid);
        screenBlocker();
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function screenBlocker() {
  const finishField = document.querySelector(".finish");
  const ring = document.querySelector(".ring");
  const screenBlocker = document.querySelector(".screen-blocker");
  ring.classList.remove("ring--active");
  finishField.classList.remove("active-popup");
  screenBlocker.classList.add("screen-blocker--active");
  ScissorsRockPaper();
}

function showExplosionAnimation(fieldElement) {
  // Create new <img> Element
  const img = document.createElement("img");
  img.src = "../img/explosion.gif";
  img.style.height = "50px";
  img.style.width = "50px";

  // Add <img> Element to fieldElement
  fieldElement.appendChild(img);

  // remove the <img> Element after animation end
  setTimeout(() => {
    fieldElement.removeChild(img);
  }, 800); // 800 Milliseconds = 0.8 sec
}

async function checkReadyToShoot(gamePlayerId) {
  const API_URL = api + gamePlayerId + "/" + gameId + "/CheckReadyToShoot";
  const test = fetch(API_URL, {
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
    .then((data) => {
      if (data.ok) {
        return true;
      }
      return false;
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
  return test;
}

function CheckIfBoardSet(gameId) {
  const API_URL = api + gameId + "/BoardState";
  return fetch(API_URL, {
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
      if (data) {
        loadGameBoard(data);
        createLoadingScreen();
        intervalid = setInterval(checkIfPlayerReady, 1000);
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function loadFiredShots(gamePlayerId) {
  const API_URL = api + gamePlayerId + "/LoadFiredShots";
  return fetch(API_URL, {
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
      if (data) {
        data.forEach((shots) => {
          const X = shots.x;
          const Y = shots.y;
          const opponentFields = document.getElementById("opponent__board");
          const opponentField = opponentFields.querySelector(
            `[data-x="${X}"][data-y="${Y}"]`
          );
          opponentField.classList.add("field--hit");
        });
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}
function loadShotsFromOpponent() {
  loadShotsFromOpponentFromTheDB(gamePlayerId);
}
function loadShotsFromOpponentFromTheDB(gamePlayerId) {
  const API_URL = api + gamePlayerId + "/LoadShotsFromOpponent";
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
      if (data) {
        data.forEach((shots) => {
          const X = shots.x;
          const Y = shots.y;
          const opponentFields = document.getElementById("game__board");
          const opponentField = opponentFields.querySelector(
            `[data-x="${X}"][data-y="${Y}"]`
          );
          opponentField.classList.add("field--hit");
        });
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function loadGameBoard(data) {
  // place the ships on the board and wait for the other player
  data.forEach((ships) => {
    let shipFound = false;
    const X = ships.x;
    const Y = ships.y;
    const Direction = ships.direction;
    const shipType = ships.name.toLowerCase();

    const ship = document.querySelector(`[data-name="${shipType}"]`);
    const shipSize = parseInt(ship.getAttribute("data-size"));
    const currentX = parseInt(X);
    const currentY = parseInt(Y);
    const screenBlocker = document.querySelector(".screen-blocker");

    for (let i = 0; i < 10 && !shipFound; i++) {
      for (let j = 0; j < 10 && !shipFound; j++) {
        if (currentX === i && currentY === j) {
          const container = document.querySelector(
            `[data-x="${i}"][data-y="${j}"]`
          );
          container.appendChild(ship);

          container.style.zIndex = 1000;

          ship.setAttribute(
            "data-direction",
            Direction === 0 ? "horizontal" : "vertical"
          );
          ship.setAttribute("draggable", false);
          ship.classList.add(Direction === 0 ? "horizontal" : "vertical");
          changeHitBoxOnClick(
            Direction === DirectionEnum.HORIZONTAL,
            currentX,
            currentY,
            shipSize,
            shipSize
          );
          shipFound = true;
        }
      }
    }
  });
}

function loadHitShips(gamePlayerId) {
  const API_URL = api + gamePlayerId + "/LoadHitShips";
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
      if (data) {
        data.forEach((shots) => {
          const X = shots.x;
          const Y = shots.y;
          const opponentFields = document.getElementById("opponent__board");
          const opponentField = opponentFields.querySelector(
            `[data-x="${X}"][data-y="${Y}"]`
          );
          opponentField.classList.add("field--hit--ship");
        });
      }
    });
}

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

function countShots() {
  const API_URL = api + gamePlayerId + "/CountShots";
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
      showCountShots(data.shots, data.nextPlayer, data.gameState);
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
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

function showCountShots(shots, nextPlayer, gameState) {
  const counter = document.querySelector(".counter");
  const animationDuration = 500;

  if (nextPlayer.toString() === gamePlayerId) {
    counter.classList.add("counter--active");
    document.querySelector(".cursor").classList.add("cursor--active");
    document.body.style.cursor = "none";
  } else {
    counter.classList.remove("counter--active");
    setTimeout(() => {
      document.querySelector(".cursor").classList.remove("cursor--active");
      document.body.style.cursor = "crosshair";
    }, animationDuration);
  }
  if (shots) {
    counter.innerHTML = shots;
  }
  if (gameState === GameStateEnum.Won || gameState === GameStateEnum.Lost) {
    connectionGameHub.stop();
    counter.classList.remove("counter--active");
    document.querySelector(".cursor").classList.remove("cursor--active");
    document.body.style.cursor = "crosshair";

    const gameEnd = document.querySelector(".container");
    gameEnd.innerHTML +=
      gameState === GameStateEnum.Won
        ? `<div class="win"><img src="../img/VictoryRoyaleSlate.png"></img></div>`
        : `<div class="lost"><img src="../img/die.png"></img></div>`;
    const result =
      gameState === GameStateEnum.Won
        ? document.querySelector(".win")
        : document.querySelector(".lost");

    document.body.style.margin = "0";
    document.body.style.overflow = "hidden";

    result.addEventListener("click", (e) => {
      result.remove();
      document.body.style.margin = "5px";
      document.body.style.overflowY = "visible";
    });
  }
}
