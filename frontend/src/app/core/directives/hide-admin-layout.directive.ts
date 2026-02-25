import { Directive, ElementRef, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Directive({
  selector: '[appHideLayoutAdmin]'
})
export class HideAdminLayoutDirective implements OnInit {

  private adminRoutes = ['/admin']; // all admin pages

  constructor(private el: ElementRef, private router: Router) {}

  ngOnInit() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        const currentUrl = event.urlAfterRedirects;

        const isAdminPage = currentUrl.startsWith('/admin');
        this.el.nativeElement.style.display = isAdminPage ? '' : 'none';
      }
    });
  }
}
