// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
const gameId = urlParams.get("gameId");
const gamePlayerId = urlParams.get("gamePlayerId");
let ifShipsPlaced = false;
let lastContainer = null;

// Informationbox
const dialogCloseButton = document.querySelector(".dialog--close");
const informationDialog = document.querySelector(".information__dialog");
const informationButton = document.querySelector(".information__button");

const muteButton = document.querySelector(".mute__button");
let mute = false;

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

const sound = new Audio("../sound/pewpew.mp3");

createBoard(myBoard, true);
createBoard(gameOpponent, false);

Promise.all([CheckIfBoardSet(gamePlayerId), loadFiredShots(gamePlayerId)]);
loadHitShips(gamePlayerId);

let zIndexChange = 1;
let currentField = null;

let intervalid;
let hoverTimer = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".own--field");
const shipSelection = document.querySelector(".ship__selection");
const opponentFields = document.querySelectorAll(".opponentField");

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
    drag(draggable, e, e);
  });

  draggable.addEventListener("touchstart", (e) => {
    dragstart(draggable, e);
  });

  draggable.addEventListener("touchmove", (e) => {
    const touch = e.touches[0];
    drag(draggable, touch, e);
  });

  draggable.addEventListener("touchend", (e) => {
    const parentElement = draggable.parentNode;
    if (parentElement.classList.contains("ship_selection")) {
      return;
    }
    dragend(draggable, e);
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

function drag(draggable, pointXY, e) {
  const xClient = pointXY.clientX;
  const yClient = pointXY.clientY;
  const xPage = pointXY.pageX;
  const yPage = pointXY.pageY;

  const container = document.elementFromPoint(xClient, yClient);
  const isPlacementValid = dragover(container, e);
  if (isPlacementValid) {
    lastContainer = container;
  }
  if (!originField.classList.contains("own--field")) {
    dragMove(draggable, xPage, yPage);
  }
}

function dragstart(draggable, e) {
  let img = new Image();
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
  const commonDivisor = draggable.offsetWidth % 52;
  // the 1x1 Ship witdh is allways 52px
  const shipWitdh = 52;

  fakeShip.classList.add("ship--active");

  fakeShip.style.backgroundImage = `url(${imgURL})`;
  fakeShip.style.width = draggable.offsetWidth + "px";
  fakeShip.style.height = draggable.offsetHeight + "px";
  fakeShip.style.backgroundSize = computedStyle.backgroundSize;
  fakeShip.style.left = x - shipWitdh / 2 - shipWitdh * commonDivisor + "px";
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
  fakeShip.classList.remove("ship--active");
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
  if (originField && originField.dataset.ships) {
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
        div.classList.add("own--field");
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
    if (futureField != null && futureField.dataset.ships > 0 + tinyShip) {
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
  const screenBlocker = document.querySelector(".screen-blocker");
  let error_popup__screen_blocker = document.querySelector(
    ".error-popup__screen-blocker"
  );
  let error_popup = document.querySelector(".error-popup");
  screenBlocker.classList.remove("screen-blocker--active");
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
  const screenBlocker = document.querySelector(".screen-blocker");
  const error_popup = document.querySelector(".error-popup");
  error_popup.classList.add("error-popup--active");
  error_popup__screen_blocker.classList.add(
    "error-popup__screen-blocker--active"
  );
  screenBlocker.classList.remove("screen-blocker--active");
  commit_button.classList.add("commit-button--active");
}

function screenBlocker(isHuman) {
  const finishField = document.querySelector(".finish");
  const ring = document.querySelector(".ring");
  const screenBlocker = document.querySelector(".screen-blocker");
  ring.classList.remove("ring--active");
  finishField.classList.remove("active-popup");
  screenBlocker.classList.add("screen-blocker--active");
  if (isHuman) {
    ScissorsRockPaper();
  }
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

function showExplosionAnimation(fieldElement) {
  // Create new <img> Element
  const img = document.createElement("img");
  img.src = "../img/explosion.gif";
  img.style.height = "50px";
  img.style.width = "50px";
  img.style.top = "1px";
  img.style.left = "1px";
  img.style.position = "absolute";
  img.style.zIndex = "4000";
  // Füge das <img> Element zum Ziel-Feld hinzu
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
        ifShipsPlaced = true;
        loadGameBoard(data);
        createLoadingScreen();
        if (!isHuman)
          setTimeout(() => {
            screenBlocker(isHuman);
            isReadyToShotBot = true;
          }, 1000);
        if (isHuman) {
          intervalid = setInterval(checkIfPlayerReady, 1000);
        }
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
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
        screenBlocker(isHuman);
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
