const follower = document.getElementById('follower');
const printout = document.getElementById('printout');

let mouseX = (e) => {
    return e.clientX;
}

let mouseY = (e) => {
    return e.clientY;
}

positionElement = (e) => {
    let mouse = {
        x: mouseX(e),
        y: mouseY(e)
    };
    follower.style.top = mouse.y + 'px';
    follower.style.left = mouse.x + 'px';
}
let time = false;
window.onmousemove = init = (e) => {
    _event = e;
    timer = setTimeout(() => {
        positionElement(_event);
    }, 1)};