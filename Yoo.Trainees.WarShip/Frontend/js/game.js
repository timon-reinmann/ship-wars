// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
console.log(urlParams.get("playerid"));
let boardState = new Array(10).fill(null).map(() => new Array(10).fill(0));
let originField = null;
let toggle = false;
const myBoard = document.getElementById("game__board");
const gameOpponent = document.getElementById("opponent__board");

createBoard(myBoard, true);
createBoard(gameOpponent, false);

function createBoard(gameBoard, isMyBoard) {
  let countingFields = 0;
  for (let y = 0; y < 10; y++) {
    for (let x = 0; x < 10; x++) {
      const div = document.createElement("div");
      div.classList.add("field");
      div.classList.add(`b${countingFields}`);
      div.setAttribute("data-x", x);
      div.setAttribute("data-y", y);
      if (isMyBoard) {
        div.classList.add("ownField");
        div.setAttribute("id", `box ${countingFields}`);
        div.setAttribute("data-size", 0);
        div.setAttribute("data-new", "false");
      }
      gameBoard.appendChild(div);
      countingFields++;
    }
  }
}

let zIndexChange = 1;
let currentField = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".ownField");
const shipSelection = document.querySelector(".ship__selection");

draggables.forEach((draggable) => {
  draggable.addEventListener("click", (e) => {
    draggable.setAttribute("data-direction", "vertical");
    toggle = draggable.classList.toggle("vertical");

    let currenShip = draggable.parentNode;
    const currentX = parseInt(currenShip.getAttribute("data-x"));
    const currentY = parseInt(currenShip.getAttribute("data-y"));
    const shipSize = parseInt(currenShip.getAttribute("data-size"));
    boardHitBoxOnClick(toggle, currentX, currentY, shipSize, 0);
    boardHitBoxOnClick(!toggle, currentX, currentY, shipSize, shipSize);
  });
  draggable.addEventListener("dragstart", (e) => {
    originField = draggable.parentNode;
    draggable.classList.add("dragging");
    let draggableParentDiv = draggable.parentNode;
    deleteShipHitBox(draggableParentDiv);
  });
  draggable.addEventListener("dragend", () => {
    draggable.classList.remove("dragging");
    currentField.style.zIndex = zIndexChange + 1;
    const currentX = parseInt(currentField.getAttribute("data-x"));
    const currentY = parseInt(currentField.getAttribute("data-y"));
    const shipSize = parseInt(draggable.getAttribute("data-size"));
    for (let i = 1; i < shipSize; i++) {
      const adjustFields = document.querySelector(
        `[data-x="${currentX + i}"][data-y="${currentY}"]`
      );
      if (adjustFields) {
        adjustFields.style.zIndex = zIndexChange;
      }
    }
    for (let i = -1; i <= shipSize; i++) {
      for (let j = -1; j < 2; j++) {
        let field = null;
        if (draggable.getAttribute("data-direction") !== "vertical") {
          field = document.querySelector(
            `[data-x="${currentX + i}"][data-y="${currentY + j}"]`
          );
        } else {
          field = document.querySelector(
            `[data-x="${currentX + j}"][data-y="${currentY + i}"]`
          );
        }
        if (field) {
          field.setAttribute("data-size", shipSize);
        }
      }
    }
    zIndexChange++;
  });
});

containers.forEach((container) => {
  container.addEventListener("dragstart", (e) => {
    container.setAttribute("data-new", "true");
  });
  container.addEventListener("dragover", (e) => {
    e.preventDefault();

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
      let checkField = null;
      if (draggable.getAttribute("data-direction") !== "vertical") {
        checkField = document.querySelector(
          `[data-x="${currentX + i}"][data-y="${currentY}"]`
        );
      } else {
        freeField = document.querySelector(
          `[data-x="${currentX}"][data-y="${currentY + i}"]`
        );
      }
      if (
        !freeField || 
        freeField.getAttribute("data-ships") > 0
      ) {
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
    let oldField = null;
    const oldX = parseInt(originField.getAttribute("data-x"));
    const oldY = parseInt(originField.getAttribute("data-y"));
    const oldShipSize = parseInt(originField.firstChild.getAttribute("data-size"));
    for (let i = -1; i <= oldShipSize; i++) {
      for (let j = -1; j < 2; j++) {
        if (
          container.firstChild.getAttribute("data-direction") !== "vertical"
        ) {
          oldField = document.querySelector(
            `[data-x="${oldX + i}"][data-y="${oldY + j}"]`
          );
        } else {
          oldField = document.querySelector(
            `[data-x="${oldX + j}"][data-y="${oldY + i}"]`
          );
        }
        if (oldField) {
          let currentShips = parseInt(oldField.getAttribute("data-ships"), 10) || 0;
          currentShips = Math.max(0, currentShips - 1);
          oldField.setAttribute("data-ships", currentShips);
          oldField.setAttribute("data-new", "false");
        }
      }
    }
  }
}

function boardHitBoxOnClick(
  toggleOnClick,
  currentX,
  currentY,
  shipSize,
  fieldSize
) {
  for (let i = -1; i <= shipSize; i++) {
    for (let j = -1; j < 2; j++) {
      let field = null;
      if(!toggleOnClick) {
        field = document.querySelector(`[data-x="${currentX + j}"][data-y="${currentY + i}"]`);
      } else { 
        field = document.querySelector(`[data-x="${currentX + i}"][data-y="${currentY + j}"]`);
      }
      if (field) {
        let currentShips = parseInt(field.getAttribute("data-ships"), 10) || 0;

        if(fieldSize > 0) {
          currentShips += 1;
        } else {
          currentShips = Math.max(0, currentShips - 1);
        }

        field.setAttribute("data-ships", currentShips);
        if(fieldSize > 0) {
          if(!toggleOnClick){
            if(field.firstChild)
              field.firstChild.setAttribute("data-direction", "vertical");
          } else {
            if(field.firstChild)
              field.firstChild.setAttribute("data-direction", "horizontal");
          } 
        }
      }
    }
  }
}

function canChangeDirection(draggable, currentX, currentY, shipSize) {
  let isValid = true;
  for (let i = 0; i < shipSize; i++) {
    let futureField = null;
    if(draggable.getAttribute("data-direction") !== "vertical") {
      futureField = document.querySelector(`[data-x="${currentX + i}"][data-y="${currentY}"]`);
    } else {
      futureField = document.querySelector(`[data-x="${currentX}"][data-y="${currentY + i}"]`);
    }
    if (!futureField || futureField.getAttribute("data-ships") > 0) {
      isValid = false;
      break;
    }
  }
  
  return isValid;
}
// ...

let ShipPosition = {
  Y: currentY,
  X: currentX,
  Direction: 1,
};
daten = JSON.stringify(ShipPositions);
("use strict");

const API_URL_Email = "https://localhost:7118/api/Game/SaveShips";
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
  body: ShipPositions,
  method: "POST",
})
  .then((response) => response.json())
  .then((data) => {
    console.log("Daten");
  })
  .catch((error) => {
    console.error("Es gab einen Fehler bei der Anfrage:", error);
  });
