import { Component } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { Router ,ActivatedRoute} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, take } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginData = { email: '', password: '' };
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService,
      private route: ActivatedRoute

  ) {}
onLogin() {
  if (!this.loginData.email || !this.loginData.password) {
    this.toastr.warning('Please fill all fields');
    return;
  }

  this.loading = true;

  this.auth.login(this.loginData).subscribe({
    next: () => {
      this.loading = false;
      this.toastr.success('Login successful');

      // ✅ FIX 4 applied
      const returnUrl =
        this.route.snapshot.queryParams['returnUrl'] ||
        this.auth.getHomeRoute();

      this.router.navigateByUrl(returnUrl);
    },
    error: (err) => {
      this.loading = false;
      this.toastr.error(err?.error?.message || 'Login failed');
    }
  });
}

 
}
