let copyText = document.querySelector(".copy-text");
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

const submit_button = document.getElementById("penis2");
submit_button.addEventListener("click", function() {
    let lobbyName = document.getElementById("penis3").value;
    lobbyName = JSON.stringify(lobbyName);

    'use strict';
    let url = "https://localhost:7118/api/Game/";
    let jsonURL = JSON.stringify(url);

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
        document.getElementById('penis').value = "https://localhost:7118/" +  data;
    })
    .catch(error => {
        console.error("Es gab einen Fehler bei der Anfrage:", error);
    });
});

