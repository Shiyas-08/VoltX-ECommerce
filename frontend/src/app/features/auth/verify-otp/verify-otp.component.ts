import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './verify-otp.component.html'
})
export class VerifyOtpComponent {

  email = this.route.snapshot.queryParamMap.get('email');

  form = this.fb.group({
    otp: ['', Validators.required]
  });

  loading = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {}

  submit() {
    if (this.form.invalid || !this.email) return;

    this.loading = true;

    this.auth.verifyOtp(this.email, this.form.value.otp!)
      .subscribe({
        next: (res:any) => {
          this.loading = false;
          this.toastr.success(res.message);
          this.router.navigate(
            ['/auth/reset-password'],
            { queryParams: { email: this.email } }
          );
        },
        error: err => {
          this.loading = false;
          this.toastr.error(err.error?.message);
        }
      });
  }
}
