const commitBtn = document.querySelector(".commit-button");
let isHuman = false;

let isReadyToShootBot = true;

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
    isReadyToShootBot = true;
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

async function sendShips(Ships) {
  const API_URL = api + "SaveShips";
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
      Ships,
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

opponentFields.forEach((opponentField) => {
  opponentField.addEventListener("click", async (e) => {
    if (!isReadyToShootBot) {
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
        if (data.hit === 1) {
          opponentField.classList.add("field--hit--ship");
          showExplosionAnimation(opponentField);
        }
        isReadyToShootBot = false;
        setTimeout(() => {
          isReadyToShootBot = true;
        }, 2500);
      });
  });
});
