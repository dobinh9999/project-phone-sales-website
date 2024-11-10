
const listimages = document.querySelector('.list-images')
const imgs = document.getElementsByTagName('img')
const btnleft = document.querySelector('.btn-left')
const btnright = document.querySelector('.btn-right')
const length = imgs.length;
let curret = 0

const handlechanlide = () => {
    if (curret == length - 1) {
        curret = 0;
        let width = imgs[0].offsetWidth
        listimages.style.transform = `translateX(${0}px)`
        document.querySelector('.active').classList.remove('active')
        document.querySelector('.index-item-' + curret).classList.add('active')
    }
    else {
        curret++;
        let width = imgs[0].offsetWidth
        listimages.style.transform = `translateX(${width * -1 * curret}px)`
        document.querySelector('.active').classList.remove('active')
        document.querySelector('.index-item-' + curret).classList.add('active')
    }
}

let handleEventchaneslide = setInterval(handlechanlide, 4000)

btnright.addEventListener('click', () => {
    clearInterval(handleEventchaneslide)
    handlechanlide()
    handleEventchaneslide = setInterval(handlechanlide, 4000)
})
btnleft.addEventListener('click', () => {
    if (curret == 0) {
        curret == length - 1
        let width = imgs[0].offsetWidth
        listimages.style.transform = `translateX(${width * -1 * curret}px)`
        document.querySelector('.active').classList.remove('active')
        document.querySelector('.index-item-' + curret).classList.add('active')
    }
    else {
        curret--;
        let width = imgs[0].offsetWidth
        listimages.style.transform = `translateX(${width * -1 * curret}px)`
        document.querySelector('.active').classList.remove('active')
        document.querySelector('.index-item-' + curret).classList.add('active')
    }
    cleaninterval(handleEventchaneslide)
    handleEventchaneslide = setInterval(handlechanlide, 4000)
})
