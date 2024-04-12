const thumImg = document.querySelectorAll(".img-small");
const imgLarge = document.querySelector(".img-main");

thumImg.forEach((img, i) => {
    i++;
    if (i === 1) {
        img.classList.add("active");
        imgLarge.src = img.src;
    }
    img.addEventListener("click", (e) => {
        imgLarge.src = img.src;
        thumImg.forEach((thumb) => thumb.classList.remove("active"));
        img.classList.add("active");
    });
});