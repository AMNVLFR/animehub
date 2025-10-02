cod// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    // Smooth scrolling for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            const targetId = this.getAttribute('href');
            if (targetId !== '#') {
                e.preventDefault();
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    targetElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // Scroll to Top Button
    const scrollToTopBtn = document.getElementById('scrollToTopBtn');
    if (scrollToTopBtn) {
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollToTopBtn.style.display = 'flex';
            } else {
                scrollToTopBtn.style.display = 'none';
            }
        });

        scrollToTopBtn.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // Anime card hover effect
    const animeCards = document.querySelectorAll('.anime-card');
    animeCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px) scale(1.02)';
        });

        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Watch button functionality
    const watchButtons = document.querySelectorAll('.card-overlay .btn');
    watchButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const animeTitle = this.closest('.anime-card').querySelector('.card-title').textContent;
            alert(`Starting "${animeTitle}"... (This would open the watch page)`);
        });
    });

    // Category buttons
    const categoryButtons = document.querySelectorAll('.btn-outline-anime');
    categoryButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            categoryButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
            // Here you would filter the anime grid based on category
            console.log('Filtering by:', this.textContent);
        });
    });

    // Dashboard stats animation
    const statNumbers = document.querySelectorAll('.stat-number');
    statNumbers.forEach(stat => {
        const targetValue = parseInt(stat.textContent);
        let currentValue = 0;
        const increment = targetValue / 50; // Adjust speed

        const timer = setInterval(() => {
            currentValue += increment;
            if (currentValue >= targetValue) {
                currentValue = targetValue;
                clearInterval(timer);
            }
            stat.textContent = Math.floor(currentValue);
        }, 30);
    });

    // Navbar toggle animation
    const navbarToggler = document.querySelector('.navbar-toggler');
    if (navbarToggler) {
        navbarToggler.addEventListener('click', function() {
            this.classList.toggle('active');
        });
    }

    // Lazy loading for images
    const images = document.querySelectorAll('img[loading="lazy"]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.classList.remove('skeleton');
                observer.unobserve(img);
            }
        });
    });

    images.forEach(img => {
        imageObserver.observe(img);
    });

    // Loading animation for images
    images.forEach(img => {
        img.addEventListener('load', function() {
            this.style.opacity = '1';
        });
        img.style.opacity = '0';
        img.style.transition = 'opacity 0.3s ease';
    });

    // Image error handling
    const allImages = document.querySelectorAll('img');
    allImages.forEach(img => {
        img.addEventListener('error', function() {
            // Create a fallback placeholder
            this.style.display = 'none';
            const placeholder = document.createElement('div');
            placeholder.style.width = '100%';
            placeholder.style.height = '100%';
            placeholder.style.background = 'linear-gradient(45deg, #2c2c2c, #3c3c3c)';
            placeholder.style.display = 'flex';
            placeholder.style.alignItems = 'center';
            placeholder.style.justifyContent = 'center';
            placeholder.style.color = '#ffffff';
            placeholder.style.fontSize = '0.9rem';
            placeholder.textContent = 'Image not available';
            this.parentNode.appendChild(placeholder);

            // Try to reload the image after a delay
            setTimeout(() => {
                this.src = this.src; // Retry loading
                this.style.display = '';
                if (placeholder.parentNode) {
                    placeholder.parentNode.removeChild(placeholder);
                }
            }, 2000);
        });
    });

    // Rating Stars Functionality
    const ratingStars = document.querySelectorAll('.rating-stars i');
    ratingStars.forEach(star => {
        star.addEventListener('click', function() {
            const rating = this.getAttribute('data-rating');
            const starsContainer = this.parentElement;

            // Remove active class from all stars in this container
            starsContainer.querySelectorAll('i').forEach(s => s.classList.remove('active'));

            // Add active class to clicked star and all before it
            for (let i = 0; i < rating; i++) {
                starsContainer.children[i].classList.add('active');
            }

            console.log('Rating set to:', rating);
        });

        star.addEventListener('mouseenter', function() {
            const rating = this.getAttribute('data-rating');
            const starsContainer = this.parentElement;

            // Preview rating on hover
            starsContainer.querySelectorAll('i').forEach((s, index) => {
                if (index < rating) {
                    s.classList.add('hover');
                } else {
                    s.classList.remove('hover');
                }
            });
        });

        star.addEventListener('mouseleave', function() {
            const starsContainer = this.parentElement;
            starsContainer.querySelectorAll('i').forEach(s => s.classList.remove('hover'));
        });
    });

    // Bookmark/Favorite/Like buttons
    const actionButtons = document.querySelectorAll('.anime-card .btn-group-sm .btn');
    actionButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();

            const icon = this.querySelector('i');
            const action = this.title.toLowerCase();

            // Toggle active state
            this.classList.toggle('active');
            if (this.classList.contains('active')) {
                if (action === 'like') {
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                    this.classList.remove('btn-outline-danger');
                    this.classList.add('btn-danger');
                } else if (action === 'bookmark') {
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                    this.classList.remove('btn-outline-warning');
                    this.classList.add('btn-warning');
                } else if (action === 'add to list') {
                    this.innerHTML = '<i class="fas fa-check"></i>';
                    this.classList.remove('btn-outline-info');
                    this.classList.add('btn-info');
                }
            } else {
                if (action === 'like') {
                    icon.classList.remove('fas');
                    icon.classList.add('far');
                    this.classList.remove('btn-danger');
                    this.classList.add('btn-outline-danger');
                } else if (action === 'bookmark') {
                    icon.classList.remove('fas');
                    icon.classList.add('far');
                    this.classList.remove('btn-warning');
                    this.classList.add('btn-outline-warning');
                } else if (action === 'add to list') {
                    this.innerHTML = '<i class="fas fa-plus"></i>';
                    this.classList.remove('btn-info');
                    this.classList.add('btn-outline-info');
                }
            }

            console.log(`${action} toggled for anime`);
        });
    });
});
