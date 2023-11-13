var video = document.getElementById("myVideo");
var playButton = document.getElementById("play-button");
const videoNice = document.querySelector(".video");

playButton.addEventListener("click", function () {
  if (video.ended) {
    video.currentTime = 0;
  }
  videoNice.classList.add("video--active");
  video.play();
  setTimeout(function () {
    videoNice.classList.remove("video--active");
  }, removeVideoTimer);
});
