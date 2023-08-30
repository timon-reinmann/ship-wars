let n = 0;
let game = document.getElementById("game__board")
for (let x = 0; x < 10; x++) {
  for (let y = 0; y < 10; y++) {
    let div = document.createElement("div");
    div.classList.add("field");
    div.classList.add(`b${n}`);
    div.setAttribute("data-x", x);
    div.setAttribute("data-y", y);
    game.appendChild(div);
    n+=1;
  }
}
'use strict';

const API_URL = 'https://localhost:7118/api/Game';
fetch(API_URL, {
    "credentials": "omit",
    "headers": {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0",
        "Accept": "*/*",
        "Accept-Language": "de,en-US;q=0.7,en;q=0.3",
        "Content-Type": "application/json",
        "Sec-Fetch-Dest": "empty",
    },
    "body": 'ERSETZTEN',
    "method": "POST",
})
.then(response => response.json())
.data.then(data => {
    console.log("Daten:", data);
})
.catch(error => {
    console.error("Es gab einen Fehler bei der Anfrage:", error);
});