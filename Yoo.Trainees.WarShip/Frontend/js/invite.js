let copyText = document.querySelector(".copy__text");
copyText.querySelector("button").addEventListener("click",function(){
    let input = copyText.querySelector("input.text");
    input.select();
    document.execCommand("copy");
    copyText.classList.add("active");
    window.getSelection().removeAllRanges();
    setTimeout(function(){
        copyText.classList.remove("active");
    },2500);
});


const submit_button = document.getElementById("lobbyinput");
submit_button.addEventListener("click", function() {

    
    let createGame = document.querySelector(".submit__lobbyname");
    createGame.classList.add("active");
    setTimeout(function(){
        createGame.classList.remove("active");
    },2500);

    let lobbyName = JSON.stringify(document.getElementById("lobbyname").value);

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
        "body": lobbyName,
        "method": "POST",
    })  
    .then(response => response.json())
    .then(data => {
        console.log("Daten:", data);
        document.getElementById('linkoutput').value = "http://127.0.0.1:5500/Frontend/html/game-pvp.html?playerid=" +  data;
    })
    .catch(error => {
        console.error("Es gab einen Fehler bei der Anfrage:", error);
    });

});

