import { Directive, ElementRef, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Directive({
  selector: '[appHideLayout]'
})
export class HideLayoutDirective implements OnInit {

  constructor(
    private el: ElementRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.updateVisibility(event.urlAfterRedirects);
      }
    });

    // handle refresh
    this.updateVisibility(this.router.url);
  }

  private updateVisibility(url: string) {

    const shouldHide =
      url.startsWith('/auth') ||     
      url.startsWith('/login') ||    
      url.startsWith('/register') || 
      url.startsWith('/admin') ||    
      url === '/not-found';          

    this.el.nativeElement.style.display = shouldHide ? 'none' : '';
  }
}
