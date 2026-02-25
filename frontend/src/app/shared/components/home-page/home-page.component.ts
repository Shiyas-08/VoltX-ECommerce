import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomeComponent {
    constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  images = [
    'assets/banner img/banner1.png',
    'assets/banner img/banner2.png',
    'assets/banner img/banner3.png'
  ];

  currentSlide = 0;
  private slideInterval: any;
    isLoading = true;

  ngOnInit() {
   if (this.auth.isAdmin()) {
      this.router.navigate(['/admin/dashboard']);
      return;
    }
     setTimeout(() => {
      this.isLoading = false;
    }, 800); 
  
    // Auto slide every 3 seconds
    this.slideInterval = setInterval(() => {
      this.nextSlide();
    }, 3000);

  }

  ngOnDestroy() {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.images.length;
  }

  prevSlide() {
    this.currentSlide =
      (this.currentSlide - 1 + this.images.length) % this.images.length;
  }
    goToSlide(index: number) {
    this.currentSlide = index;
  }
}


