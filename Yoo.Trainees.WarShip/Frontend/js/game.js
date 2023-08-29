let n = 0;
let game = document.getElementsByClassName("game__board")
for (let x = 0; x < 10; x++) {
    for (let y = 0; y < 10; y++) {
      let div = document.createElement("div");
      game.appendChild(div);
      div.classList.add("field");
      div.classList.add(`b${n}`);
      div.setAttribute("data-x", x);
      div.setAttribute("data-y", y);
      document.body.appendChild(div);
      n+=1;
    }
  }