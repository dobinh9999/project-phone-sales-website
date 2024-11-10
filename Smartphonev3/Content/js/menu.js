const menuToggle = document.querySelector('.menu-toggle');
const menuContent = document.querySelector('.menu-content');
const subMenuContents = document.querySelectorAll('.sub-menu-content');
let timeoutId;
let isSubMenuVisible = false; // Trạng thái hiển thị của sub-menu

if (menuToggle && menuContent) { // Kiểm tra xem menuToggle và menuContent có tồn tại không
    // Toggle menu visibility on click for the main menu
    menuToggle.addEventListener('click', () => {
        menuContent.style.display = (menuContent.style.display === 'block') ? 'none' : 'block';

        // Nếu menu hiện ra, tự động ẩn sau 4 giây nếu chuột không ở trong menu
        if (menuContent.style.display === 'block') {
            setTimeout(() => {
                if (!menuContent.matches(':hover')) {
                    menuContent.style.display = 'none';
                }
            }, 4000);
        }
    });
}

// Function to hide sub-menu after 2 seconds
function hideSubMenuAfterDelay(subMenu) {
    clearTimeout(timeoutId); // Xóa bất kỳ timer nào trước đó
    timeoutId = setTimeout(() => {
        subMenu.style.display = 'none';
        isSubMenuVisible = false; // Cập nhật trạng thái là không hiển thị
    }, 2000);
}

// Function to hide all sub-menus
function hideAllSubMenus() {
    subMenuContents.forEach(subMenu => {
        subMenu.style.display = 'none';
    });
    isSubMenuVisible = false;
}

subMenuContents.forEach(subMenu => {
    const itemTitle = subMenu.previousElementSibling; // Lấy thẻ tiêu đề

    if (itemTitle && subMenu) { // Kiểm tra xem itemTitle và subMenu có tồn tại không
        itemTitle.addEventListener('click', () => {
            if (isSubMenuVisible && subMenu.style.display === 'block') {
                subMenu.style.display = 'none'; // Ẩn sub-menu khi nhấp lại
                isSubMenuVisible = false; // Cập nhật trạng thái
            } else {
                hideAllSubMenus(); // Ẩn tất cả sub-menus khác trước khi hiển thị cái mới
                subMenu.style.display = 'block'; // Hiển thị sub-menu
                isSubMenuVisible = true; // Cập nhật trạng thái
            }
            clearTimeout(timeoutId); // Xóa timer khi nhấp vào
        });

        itemTitle.addEventListener('mouseenter', () => {
            hideAllSubMenus(); // Ẩn tất cả sub-menus khác khi di chuột vào item mới
            subMenu.style.display = 'block';
            isSubMenuVisible = true; // Cập nhật trạng thái
            clearTimeout(timeoutId); // Xóa timer khi chuột vào
        });

        itemTitle.addEventListener('mouseleave', () => {
            hideSubMenuAfterDelay(subMenu); // Gọi hàm ẩn sub-menu sau 2 giây
        });

        subMenu.addEventListener('mouseenter', () => {
            clearTimeout(timeoutId); // Duy trì sub-menu hiển thị nếu chuột vào
        });

        subMenu.addEventListener('mouseleave', () => {
            hideSubMenuAfterDelay(subMenu); // Gọi hàm ẩn sub-menu sau 2 giây
        });
    }
});
