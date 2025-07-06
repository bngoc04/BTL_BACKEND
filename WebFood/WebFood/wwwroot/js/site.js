// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Initialize AOS (Animate on Scroll)
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-in-out',
            once: true,
            mirror: false
        });
    }

    // Add hover effects to cards
    const cards = document.querySelectorAll('.card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px)';
            this.style.boxShadow = '0 15px 30px rgba(255, 107, 157, 0.2)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = '';
            this.style.boxShadow = '';
        });
    });

    // Add sparkle effect to buttons
    const buttons = document.querySelectorAll('.btn-primary, .btn-success');
    buttons.forEach(button => {
        button.addEventListener('mouseover', function(e) {
            createSparkle(e, this);
        });
    });

    // Add smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });

    // Show success messages with animation
    const successMessages = document.querySelectorAll('.alert-success');
    successMessages.forEach(message => {
        message.classList.add('animate__animated', 'animate__fadeIn');
    });

    // Add floating effect to feature icons
    const featureIcons = document.querySelectorAll('.feature-icon');
    featureIcons.forEach(icon => {
        setInterval(() => {
            icon.classList.toggle('animate__pulse');
        }, 2000);
    });

    // Initialize quantity selectors
    initQuantitySelectors();

    // Add to cart animation
    const addToCartButtons = document.querySelectorAll('button[type="submit"]');
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function() {
            const form = this.closest('form');
            if (form && form.action.includes('ThemVaoGioHang')) {
                const productCard = this.closest('.card');
                if (productCard) {
                    animateAddToCart(productCard);
                }
            }
        });
    });
});

// Helper function to create sparkle effect
function createSparkle(e, element) {
    const sparkle = document.createElement('div');
    sparkle.className = 'sparkle';
    
    const size = Math.random() * 20 + 10;
    const rect = element.getBoundingClientRect();
    
    sparkle.style.width = `${size}px`;
    sparkle.style.height = `${size}px`;
    sparkle.style.left = `${e.clientX - rect.left}px`;
    sparkle.style.top = `${e.clientY - rect.top}px`;
    
    element.appendChild(sparkle);
    
    setTimeout(() => {
        sparkle.remove();
    }, 1000);
}

// Helper function for quantity selectors
function initQuantitySelectors() {
    document.querySelectorAll('.quantity-selector').forEach(selector => {
        const minusBtn = selector.querySelector('.quantity-minus');
        const plusBtn = selector.querySelector('.quantity-plus');
        const input = selector.querySelector('input');
        
        if (minusBtn && plusBtn && input) {
            minusBtn.addEventListener('click', () => {
                const value = parseInt(input.value);
                if (value > 1) {
                    input.value = value - 1;
                }
            });
            
            plusBtn.addEventListener('click', () => {
                const value = parseInt(input.value);
                input.value = value + 1;
            });
        }
    });
}

// Animation for adding to cart
function animateAddToCart(productCard) {
    // Create a clone of the product image
    const productImg = productCard.querySelector('.card-img-top');
    if (!productImg) return;
    
    const imgClone = document.createElement('img');
    imgClone.src = productImg.src;
    imgClone.style.position = 'fixed';
    imgClone.style.height = '100px';
    imgClone.style.width = 'auto';
    imgClone.style.zIndex = '1000';
    imgClone.style.borderRadius = '8px';
    imgClone.style.boxShadow = '0 5px 15px rgba(0,0,0,0.1)';
    
    const imgRect = productImg.getBoundingClientRect();
    imgClone.style.left = `${imgRect.left}px`;
    imgClone.style.top = `${imgRect.top}px`;
    
    document.body.appendChild(imgClone);
    
    // Find the cart icon position
    const cartIcon = document.querySelector('.bi-cart3');
    if (!cartIcon) {
        imgClone.remove();
        return;
    }
    
    const cartRect = cartIcon.getBoundingClientRect();
    
    // Animate the clone to the cart
    imgClone.style.transition = 'all 0.8s ease-in-out';
    setTimeout(() => {
        imgClone.style.left = `${cartRect.left}px`;
        imgClone.style.top = `${cartRect.top}px`;
        imgClone.style.height = '20px';
        imgClone.style.opacity = '0';
    }, 10);
    
    // Remove the clone after animation
    setTimeout(() => {
        imgClone.remove();
    }, 800);
}

// Show notification
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type} animate__animated animate__fadeInRight`;
    notification.innerHTML = `<div class="notification-content">${message}</div>`;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.classList.remove('animate__fadeInRight');
        notification.classList.add('animate__fadeOutRight');
        setTimeout(() => {
            notification.remove();
        }, 500);
    }, 3000);
}
