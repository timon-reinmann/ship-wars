let isHuman = false;

let showExposion = false;

let isReadyToShotBot = false;

CheckIfBoardSet(gamePlayerId).then(() => {
  if (ifShipsPlaced) {
    Promise.all([
      loadHitShips(gamePlayerId),
      loadFiredShots(gamePlayerId),
      loadShotsFromBot(gamePlayerId, showExposion),
    ]).then();
  }
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
  createLoadingScreen();
  setTimeout(() => {
    screenBlocker(isHuman);
    isReadyToShotBot = readyToShot(isReadyToShotBot);
  }, 1000);
}

commit_button.addEventListener("click", () => {
  let ship_selector = document.querySelector(".ship__selection");
  if (ship_selector.children.length === 0) {
    commitShips(commit_button);
  } else {
    error_popup(commit_button);
  }
});

async function sendShips(ships) {
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
      gamePlayerId,
      ships,
    }),
    method: "POST",
  })
    .then((data) => {
      if (data.ok) {
        ifShipsPlaced = true;
      } else {
        error_popup(commit_button);
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function getShotsFromBot(gamePlayerId, showExposion) {
  const API_URL = api + gamePlayerId + "/GetShotsFromBot";
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
        let shots = data.botShots;
        const X = shots.x;
        const Y = shots.y;
        const ownFields = document.getElementById("game__board");
        const ownField = ownFields.querySelector(
          `[data-x="${X}"][data-y="${Y}"]`
        );
        showExposion = true;
        proveIfShipHit(X, Y, gamePlayerId, ownField, showExposion);
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}

function readyToShot(isReadyToShotBot) {
  isReadyToShotBot = true;
  document.querySelector(".counter").classList.add("counter--active");
  document.querySelector(".cursor").classList.add("cursor--active");
  document.body.style.cursor = "none";
  return isReadyToShotBot;
}

function notReadyToShot(isReadyToShotBot) {
  isReadyToShotBot = false;
  document.querySelector(".counter").classList.remove("counter--active");
  document.querySelector(".cursor").classList.remove("cursor--active");
  document.body.style.cursor = "crosshair";
  return isReadyToShotBot;
}

opponentFields.forEach((opponentField) => {
  opponentField.addEventListener("click", async (e) => {
    if (!isReadyToShotBot) {
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
      body: JSON.stringify({ X: currentX, Y: currentY, gameId }),
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
        //ToDo
        if (data.hit === 1 || data.hit === 0) {
          opponentField.classList.add("field--hit");
          countShots(gamePlayerId);
        }
        if (data.hit === 1) {
          opponentField.classList.add("field--hit--ship");
          showExplosionAnimation(opponentField);
        }
        isReadyToShotBot = notReadyToShot(isReadyToShotBot);
        setTimeout(() => {
          getShotsFromBot(gamePlayerId, getShotsFromBot);
        });
      });
  });
});

function proveIfShipHit(X, Y, gamePlayerId, field, showExposion) {
  const API_URL = api + gamePlayerId + "/CheckBotHitShip";
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
    body: JSON.stringify({ X: X, Y: Y }),
    method: "POST",
  })
    .then((response) => response.json())
    .then((data) => {
      sound.play();
      //ToDo
      if (data.hit === 1 || data.hit === 0) {
        field.classList.add("field--hit");
        countShots(gamePlayerId);
        // ownField.classList.add("field--hit");
      }
      if (data.hit === 1) {
        // ownField.classList.add("field--hit--ship");
        field.classList.add("field--hit--ship");
        if (showExposion) {
          showExplosionAnimation(field);
        }
      }
      isReadyToShotBot = readyToShot(isReadyToShotBot);
    });
}

function loadShotsFromBot(gamePlayerId, showExposion) {
  const API_URL = api + gamePlayerId + "/LoadShotsFromBot";
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
        data.botShots.forEach((shots) => {
          const X = shots.x;
          const Y = shots.y;
          showExposion = false;
          const ownFields = document.getElementById("game__board");
          const ownField = ownFields.querySelector(
            `[data-x="${X}"][data-y="${Y}"]`
          );
          proveIfShipHit(X, Y, gamePlayerId, ownField, showExposion);
        });
      }
    })
    .catch((error) => {
      console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
}
