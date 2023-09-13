// Read playerid from URL
const urlParams = new URLSearchParams(window.location.search);
console.log(urlParams.get("playerid"));
let countingFields = 0;
let gameBoard = document.getElementById("game__board");
let boardState = new Array(10).fill(null).map(() => new Array(10).fill(0));
let originField = null;
let currentX = null;
let currentY = null;

for (let y = 0; y < 10; y++) {
  for (let x = 0; x < 10; x++) {
    let div = document.createElement("div");
    div.classList.add("field");
    div.classList.add("ownField");
    div.classList.add(`b${countingFields}`);
    div.setAttribute("id", `box ${countingFields}`);
    div.setAttribute("data-x", x);
    div.setAttribute("data-y", y);
    div.setAttribute("data-size", 0);
    div.setAttribute("data-new", "false");
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

let zIndexChange = 1;
let currentField = null;

const draggables = document.querySelectorAll(".ship");
const containers = document.querySelectorAll(".ownField");
const shipSelection = document.querySelector(".ship__selection");

draggables.forEach((draggable) => {
  draggable.addEventListener("click", e => {
    draggable.setAttribute("data-direction", "vertical");
    draggable.classList.toggle("vertical");
  });
  draggable.addEventListener("dragstart", (e) => {
    originField = draggable.parentNode; 
    draggable.classList.add("dragging");
  });
  draggable.addEventListener("dragend", () => {
    draggable.classList.remove("dragging");
    currentField.style.zIndex = zIndexChange + 1;
    if(draggable.classList.contains("vertical")) {
      currentY = parseInt(currentField.getAttribute("data-x"));
      currentX = parseInt(currentField.getAttribute("data-y"));
    } else {
      currentX = parseInt(currentField.getAttribute("data-x"));
      currentY = parseInt(currentField.getAttribute("data-y"));
    }
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
        for(let j = -1; j < 2; j++) {
          const field = document.querySelector(
            `[data-x="${currentX + i}"][data-y="${currentY + j}"]`
          );
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
    currentX = parseInt(container.getAttribute("data-x"));
    currentY = parseInt(container.getAttribute("data-y"));
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
      const checkField = document.querySelector(
        `[data-x="${currentX + i}"][data-y="${currentY}"]`
      );
      
      if (
        !checkField || 
        (checkField.getAttribute("data-size") > 0 && 
        draggable.id != checkField.querySelector(".ship").id) || 
        (checkField.querySelector(".ship") &&
        draggable.id != checkField.querySelector(".ship").id)
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
      currentField = container;  // Aktualisiere das aktuelle linke Feld
      draggable.classList.remove("invalid");
    } else {
      // Das Schiff kann nicht platziert werden
      draggable.classList.add("invalid");
    }
  });
  // Komischer Weise geht das auch mit drag anstatt drop
  container.addEventListener("drop", (e) => {
    e.preventDefault();
    if(originField) {
      const oldX = parseInt(originField.getAttribute("data-x"));
      const oldY = parseInt(originField.getAttribute("data-y"));
      const oldShipSize = parseInt(originField.getAttribute("data-size"));
      for (let i = -1; i <= oldShipSize; i++) {
        for(let j = -1; j < 2; j++) {
          const oldField = document.querySelector(`[data-x="${oldX + i}"][data-y="${oldY + j}"]`);
          if (oldField) {
            oldField.setAttribute("data-size", 0);
            oldField.setAttribute("data-new", "false");
          }
        }
      }
      container.setAttribute("data-size", 0);
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
