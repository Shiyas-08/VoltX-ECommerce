import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/core/services/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  loading = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  submit() {
    if (this.form.invalid) return;

    this.loading = true;

    this.auth.forgotPassword(this.form.value.email!)
      .subscribe({
        next: (res:any) => {
          this.loading = false;
          this.toastr.success(res.message);
          this.router.navigate(
            ['/auth/verify-otp'],
            { queryParams: { email: this.form.value.email } }
          );
        },
        error: err => {
          this.loading = false;
          this.toastr.error(err.error?.message);
        }
      });
  }
}
