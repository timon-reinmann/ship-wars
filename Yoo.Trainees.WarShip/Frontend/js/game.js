// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
console.log(urlParams.get("playerid"));
let countingFields = 0;
let gameBoard = document.getElementById("game__board");
let boardState = new Array(10).fill(null).map(() => new Array(10).fill(0));


for (let y = 0; y < 10; y++) {
  for (let x = 0; x < 10; x++) {
    let div = document.createElement("div");
    div.classList.add("field");
    div.classList.add("ownField");
    div.classList.add(`b${countingFields}`);
    div.setAttribute("id", `box ${countingFields}`);
    div.setAttribute("data-x", x);
    div.setAttribute("data-y", y);
    gameBoard.appendChild(div);
    countingFields += 1;
  }
}

countingFields = 0;
let gameOpponent = document.getElementById("opponent__board");

for (let x = 0; x < 10; x++) {
  for (let y = 0; y < 10; y++) {
    let div = document.createElement("div");
    div.classList.add("field");
    div.classList.add(`b${countingFields}`);
    div.setAttribute("data-x", x);
    div.setAttribute("data-y", y);
    gameOpponent.appendChild(div);
    countingFields += 1;
  }
}

let dragStartX = 0;
let shipOffsetX = 0;
let zIndexChange = 1;
let currentField = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".ownField");
const shipSelection = document.querySelector(".ship__selection");

draggables.forEach((draggable) => {
  draggable.addEventListener("dragstart", (e) => {
    dragStartX = e.clientX;
    shipOffsetX = dragStartX - draggable.getBoundingClientRect().left;
    draggable.classList.add("dragging");
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
    zIndexChange++;
  });
});

// containers.forEach((container) => {
//   container.addEventListener("dragover", (e) => {
//     e.preventDefault();

//     const draggable = document.querySelector(".dragging");
//     const shipSize = parseInt(draggable.getAttribute("data-size"));

//     const currentX = parseInt(container.getAttribute("data-x"));

//     if (currentX + shipSize <= 10) {
//       container.appendChild(draggable);
//       currentField = container;
//       draggable.classList.remove("invalid");
//     } else {
//       draggable.classList.add("invalid");
//     }
//   });
// });

// ...

containers.forEach((container) => {
  container.addEventListener("dragover", (e) => {
    e.preventDefault();
    const draggable = document.querySelector(".dragging");
    if (!draggable) return;

    const shipSize = parseInt(draggable.getAttribute("data-size"));
    const currentX = parseInt(container.getAttribute("data-x"));
    const currentY = parseInt(container.getAttribute("data-y"));
    let shipCheck = 0;
    for (let i = currentX - 1; i >= 0; i--) {
      const checkField = document.querySelector(
        `[data-x="${i}"][data-y="${currentY}"]`
      );
      if (checkField) {
        const dataCheck = parseInt(checkField.getAttribute("data-size"));
        if (!isNaN(dataCheck)) {
          shipCheck = dataCheck;
          console.log(shipCheck + "Hallo");
        }
      }
    }
    // Überprüfen, ob genug Platz für das Schiff vorhanden ist
    let isPlacementValid = true;

    for (let i = 0; i < shipSize; i++) {
      const checkField = document.querySelector(
        `[data-x="${currentX + i}"][data-y="${currentY}"]`
      );
      if (
        !checkField && 
        (checkField.querySelector(".ship") &&
          draggable.id != checkField.querySelector(".ship").id)
      ) {
        // Es gibt ein Hindernis auf dem Platz oder der Platz ist außerhalb des Spielfelds
        isPlacementValid = false;
        break;
      }
    }    

    if (isPlacementValid) {
      // Das Schiff kann platziert werden
      
      // Falls ein altes Feld existiert, setze dessen data-size und der anderen Felder auf 0
      if (currentField) {
        const oldX = parseInt(currentField.getAttribute("data-x"));
        const oldY = parseInt(currentField.getAttribute("data-y"));
        const oldShipSize = parseInt(currentField.getAttribute("data-size"));
        for (let i = 0; i < oldShipSize; i++) {
          const oldField = document.querySelector(`[data-x="${oldX + i}"][data-y="${oldY}"]`);
          if (oldField) {
            oldField.setAttribute("data-size", 0);
          }
        }
      }
      
      // Platziere das Schiff und setze data-size für alle belegten Felder
      container.appendChild(draggable);
      for (let i = 0; i < shipSize; i++) {
        const field = document.querySelector(`[data-x="${currentX + i}"][data-y="${currentY}"]`);
        if (field) {
          field.setAttribute("data-size", shipSize);
        }
      }
      currentField = container;  // Aktualisiere das aktuelle linke Feld
      draggable.classList.remove("invalid");
    } else {
      // Das Schiff kann nicht platziert werden
      draggable.classList.add("invalid");
    }
  });
});

shipSelection.addEventListener("dragover", (e) => {
  e.preventDefault();
  const draggable = document.querySelector(".dragging");
  shipSelection.appendChild(draggable);
  currentField = null;
});

// ...

("use strict");

const API_URL = "https://localhost:7118/api/Game";
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
  body: "ERSETZTEN",
  method: "POST",
})
  .then((response) => response.json())
  .data.then((data) => {})

  .catch((error) => {
    console.error("Es gab einen Fehler bei der Anfrage:", error);
  });
