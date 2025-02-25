const dangkibtn = document.querySelector('#modal')

const trolaiclose = document.querySelector('#modal-close')

const dangnhapbtn = document.querySelector('#modal2')

const dangnhapclose = document.querySelector('#modal-close-2')



//  

dangkibtn.addEventListener("click", function () {
    document.querySelector('.modal').style.display = "flex"

})

trolaiclose.addEventListener("click", function () {
    document.querySelector('.modal').style.display = "none"

})

dangnhapbtn.addEventListener("click", function () {
    document.querySelector('.modal2').style.display = "flex"

})

dangnhapclose.addEventListener("click", function () {
    document.querySelector('.modal2').style.display = "none"

})



// slider............................ 

const rightbtn = document.querySelector('.fa-chevron-right')

const leftbtn = document.querySelector('.fa-chevron-left')

const imgnumber = document.querySelectorAll('.slider-content-left-top img')

let index = 0

rightbtn.addEventListener("click", function () {
    index = index + 1
    if (index > imgnumber.length - 1) {
        index = 0
    }
    document.querySelector(".slider-content-left-top").style.right = index * 100 + "%"


})

leftbtn.addEventListener("click", function () {
    index = index - 1
    if (index < 0) {
        index = imgnumber.length - 1
    }
    document.querySelector(".slider-content-left-top").style.right = index * 100 + "%"

})

// slider tự động hóa........ 



const imgnumberli = document.querySelectorAll('.slider-content-left-bottom li')

imgnumberli.forEach(function (image, index) {
    image.addEventListener("click", function () {
        removeactive()
        document.querySelector(".slider-content-left-top").style.right = index * 100 + "%"

        image.classList.add("active")
    })

})

function removeactive() {
    let imgauto = document.querySelector('.active')
    imgauto.classList.remove("active")

}

// slider tự chạy 



function imgAuto() {
    index = index + 1
    if (index > imgnumber.length - 1) {
        index = 0
    }
    removeactive()
    document.querySelector(".slider-content-left-top").style.right = index * 100 + "%"
    imgnumberli[index].classList.add("active")

}

setInterval(imgAuto, 3000)



// slider 4 nè 

const rightbtn4 = document.querySelector('.fa-chevron-right')

const leftbtn4 = document.querySelector('.fa-chevron-left')

const imgnumber4 = document.querySelectorAll('.slider4-container-content-left-top img')

let index4 = 0

rightbtn4.addEventListener("click", function () {
    index4 = index4 + 1
    if (index4 > imgnumber4.length - 1) {
        index4 = 0
    }
    document.querySelector(".slider4-container-content-left-top").style.right = index4 * 100 + "%"


})

leftbtn4.addEventListener("click", function () {
    index4 = index4 - 1
    if (index4 < 0) {
        index4 = imgnumber4.length - 1
    }
    document.querySelector(".slider4-container-content-left-top").style.right = index4 * 100 + "%"

})



// slider4 tự động hóa 



const imgnumberli4 = document.querySelectorAll('.slider4-container-content-left-bottom li')

imgnumberli4.forEach(function (image, index4) {
    image.addEventListener("click", function () {
        removeactive4()
        document.querySelector(".slider4-container-content-left-top").style.right = index4 * 100 + "%"

        image.classList.add("active1")
    })

})

function removeactive4() {
    let imgauto4 = document.querySelector('.active1')
    imgauto4.classList.remove("active1")

}

// slider4 tự chạy 

function imgAuto4() {
    index4 = index4 + 1
    if (index4 > imgnumber4.length - 1) {
        index4 = 0
    }
    removeactive4()
    document.querySelector(".slider4-container-content-left-top").style.right = index4 * 100 + "%"
    imgnumberli4[index4].classList.add("active1")

}

setInterval(imgAuto4, 3000)








