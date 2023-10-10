// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
const gameId = urlParams.get("playerid");
let boardState = new Array(10).fill(null).map(() => new Array(10).fill(0));
let originField = null;
let toggle = false;
let myBoard = document.getElementById("game__board");
let gameOpponent = document.getElementById("opponent__board");

createBoard(myBoard, true);
createBoard(gameOpponent, false);

let zIndexChange = 1;
let currentField = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".ownField");
const shipSelection = document.querySelector(".ship__selection");

draggables.forEach((draggable) => {
  draggable.addEventListener("click", (e) => {
    let currentShip = draggable.parentNode;
    const currentX = parseInt(currentShip.getAttribute("data-x"));
    const currentY = parseInt(currentShip.getAttribute("data-y"));
    const shipSize = parseInt(currentShip.firstChild.getAttribute("data-size"));
    const isValid = canChangeDirection(
      draggable,
      currentX,
      currentY,
      parseInt(draggable.getAttribute("data-size"))
    );
    console.log(isValid);
    if (isValid) {
      draggable.setAttribute("data-direction", "vertical");
      toggle = draggable.classList.toggle("vertical");
      changeHitBoxOnClick(toggle, currentX, currentY, shipSize, 0);
      changeHitBoxOnClick(!toggle, currentX, currentY, shipSize, shipSize);
    }
  });

  draggable.addEventListener("dragstart", e => {
    originField = draggable.parentNode;
    draggable.classList.add("dragging");
    deleteShipHitBox(draggable.parentNode);
  });

  draggable.addEventListener("dragend", () => {
    draggable.classList.remove("dragging");
    currentField.style.zIndex = zIndexChange + 1;
    const currentX = parseInt(currentField.getAttribute("data-x"));
    const currentY = parseInt(currentField.getAttribute("data-y"));
    const shipSize = parseInt(draggable.getAttribute("data-size"));
    for (let i = -1; i <= shipSize; i++) {
      for (let j = -1; j < 2; j++) {
        const field = draggable.getAttribute("data-direction") !== "vertical"
          ? document.querySelector(`[data-x="${currentX + i}"][data-y="${currentY + j}"]`)
          : document.querySelector(`[data-x="${currentX + j}"][data-y="${currentY + i}"]`);
        if (field) {
          field.setAttribute(
            "data-ships",
            parseInt(field.getAttribute("data-ships")) + 1
          );
        }
      }
    }
    zIndexChange++;
  });
});

containers.forEach((container) => {
  container.addEventListener("dragover", (e) => {
    e.preventDefault();
    container.style.zIndex = 0;
    const draggable = document.querySelector(".dragging");
    if (!draggable) return;

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
    // Überprüfen, ob genug Platz für das Schiff vorhanden ist
    let isPlacementValid = true;

    for (let i = 0; i < shipSize; i++) {
      const freeField = draggable.dataset.direction !== "vertical"
      ? document.querySelector(`[data-x="${currentX + i}"][data-y="${currentY}"]`)
      : document.querySelector(`[data-x="${currentX}"][data-y="${currentY + i}"]`);

      if (!freeField || freeField.getAttribute("data-ships") > 0) {
        // Es gibt ein Hindernis auf dem Platz oder der Platz ist außerhalb des Spielfelds
        isPlacementValid = false;
        break;
      }
      if (shipCheck > 0) {
        isPlacementValid = false;
      }
    }
    if (isPlacementValid) {
      // Falls ein altes Feld existiert, setze dessen data-size und der anderen Felder auf

      // Platziere das Schiff und setze data-size für alle belegten Felder
      container.appendChild(draggable);
      currentField = container; // Aktualisiere das aktuelle linke Feld
      draggable.classList.remove("invalid");
    } else {
      // Das Schiff kann nicht platziert werden
      draggable.classList.add("invalid");
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
function deleteShipHitBox(container) {
  if (originField) {
    const oldX = parseInt(originField.dataset.x);
    const oldY = parseInt(originField.dataset.y);
    const oldShipSize = parseInt(originField.firstChild?.dataset.size);
    const startingPoint = -1; // -1, weil das Schiff auch die Herumliegenden Felder belegt
    const shipWidth = 2; // 2 weil das Schiff immer gleich breit ist (horizontal und vertikal)
    const isVertical = container.firstChild?.dataset.direction === "vertical";
    for (let i = startingPoint; i <= oldShipSize; i++) {
      for (let j = startingPoint; j < shipWidth; j++) {
        const oldField = document.querySelector(`[data-x="${oldX + (!isVertical ? i : j)}"][data-y="${oldY + (!isVertical ? j : i)}"]`);
        if (oldField) {
          const currentShips = parseInt(oldField.dataset.ships, 10) || 0;
          oldField.dataset.ships = Math.max(0, currentShips - 1);
        }
      }
    }
  }
}

function changeHitBoxOnClick(toggleOnClick, currentX, currentY, shipSize, fieldSize) {
  for (let i = -1; i <= shipSize; i++) {
    for (let j = -1; j < 2; j++) {
      const field = document.querySelector(`[data-x="${currentX + (toggleOnClick ? i : j)}"][data-y="${currentY + (toggleOnClick ? j : i)}"]`);
      if (field) {
        const currentShips = parseInt(field.dataset.ships, 10) || 0;
        field.dataset.ships = Math.max(0, currentShips + (fieldSize > 0 ? 1 : -1));
        if (fieldSize > 0 && field.firstChild) {
          field.firstChild.dataset.direction = toggleOnClick ? "horizontal" : "vertical";
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
      gameBoard.appendChild(div);
      countingFields += 1;
    }
  }
}

function canChangeDirection(draggable, currentX, currentY, shipSize) {
  const nextPossibleField = 2;
  const isVertical = draggable.dataset.direction === "vertical";
  const highestPossibleField = shipSize == 2 ? 3 : shipSize; // That because if the Ship is 2 long it doesnt do the for loop and its alway valid

  for (let i = nextPossibleField; i < highestPossibleField; i++) {
    const x = !isVertical ? currentX : currentX + i;
    const y = !isVertical ? currentY + i : currentY;
    const futureField = document.querySelector(`[data-x="${x}"][data-y="${y}"]`);
    if (futureField.dataset.ships > 0) {
      return false; // Is not valid
    }
  }
  return true; // Is valid
}
// ...
let commit_button = document.querySelector(".commit-button");
commit_button.addEventListener("click", () => {
  let ship_selector = document.querySelector(".ship__selection");
  if (ship_selector.children.length == 0) {
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
  let GamePlayerId = "382DE87E-D0BE-4AEA-B0A8-115F08465439"; //ToDo: No hardcoding!!! 🙀
  const API_URL = "https://localhost:7118/api/Game/" + gameId + "/SaveShips";
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
    body: JSON.stringify({ Ships, GamePlayerId }),
    method: "POST",
  });
}

async function commitShips(commit_button) {
  const finishField = document.querySelector(".finish");
  const ships = document.getElementsByClassName("ship");
  const ship_positions = Array.from(ships).map(ship => ({
    ShipType: ship?.dataset.name,
    X: ship?.parentNode?.dataset.x,
    Y: ship?.parentNode?.dataset.y,
    Direction: ship?.dataset.direction,
    Id: ship?.Id,
  }));
  try {
    await sendShips(ship_positions);
    finishField.classList.add("active-popup");
    commit_button.classList.add("commit-button--active");
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