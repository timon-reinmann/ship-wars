// TODO : #Trainee-177
const follower = document.getElementById('follower');
const printout = document.getElementById('printout');

const mouseX = (e) => {
    return e.clientX;
}

const mouseY = (e) => {
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