window.addEventListener("DOMContentLoaded", () => {
  const sidebarBtn = document.getElementById("sidebarBtn");
  const sidebarBtnClose = document.getElementById("sidebarBtn-close");
  const body = document.querySelector("body");

  sidebarBtn.addEventListener("click", (e) => {
    e.preventDefault;
    let existingSidebar = document.getElementById("sidebar");
    let slideIn = document.querySelector(".slide-in");
    const closeSidebar = document.querySelector(".sidebar-close");

    if (slideIn) {
      removeSidebar(existingSidebar);
      closeSidebar.classList.remove("sidebar-close--active");
    } else {
      closeSidebar.classList.add("sidebar-close--active");
      showSidebar(sidebar);
    }
  });

  sidebarBtnClose.addEventListener("click", (e) => {
    e.preventDefault;
    let existingSidebar = document.getElementById("sidebar");
    const closeSidebar = document.querySelector(".sidebar-close");
    closeSidebar.classList.remove("sidebar-close--active");
    removeSidebar(existingSidebar);
  });

  body.addEventListener("click", (e) => {
    if (e.target.id === "faded-background") {
      // remove modal
      let modal = document.getElementById("modal");
      fadedBackground();
      modal.remove();
    }
  });

  function fadedBackground() {
    let faded = document.getElementById("faded-background");
    if (faded) {
      faded.remove();
    } else {
      faded = document.createElement("main");
      faded.id = "faded-background";
      body.append(faded);
    }
  }

  function showSidebar(sidebar) {
    sidebar.className = "slide-in";
  }

  function removeSidebar(sidebar) {
    sidebar.className = "slide-out";
    // setTimeout(() => sidebar.remove(), 775);
  }
});

//-------------------------------------------------------------
const openModal = document.querySelectorAll(".open-button");
const closeModal = document.querySelectorAll(".close-button");

closeModal.forEach(EventListernerClose);
openModal.forEach(EventListernerOpen);

function EventListernerOpen(item) {
  item.addEventListener("click", (e) => {
    e.target.parentElement.querySelector("dialog").show();
  });
}
function EventListernerClose(Item) {
  Item.addEventListener("click", (e) => {
    debugger;
    e.target.parentElement.close();
  });
}

const timer = 199200;
