const cursor = document.querySelector('.cursor');
cursor.style.pointerEvents = "none";

document.addEventListener('mousemove', e => {
    cursor.setAttribute("style", "top: "+(e.pageY - 25)+"px; left: "+(e.pageX - 25)+"px;")
    const elemBelow = document.elementFromPoint(event.clientX, event.clientY);
  
    if (elemBelow && elemBelow.clickable) {  // Ersetze "clickable" durch die Bedingung, die für dich zutrifft
        // Setze einen niedrigeren z-index, damit das Element darunter klickbar ist
        cursor.style.zIndex = "1";
    } else {
        // Setze einen höheren z-index, damit der Cursor sichtbar ist
        cursor.style.zIndex = "1000";
    }
});