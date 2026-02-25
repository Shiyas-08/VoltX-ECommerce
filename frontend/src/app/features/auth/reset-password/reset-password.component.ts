import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent {

  email = this.route.snapshot.queryParamMap.get('email');

  form = this.fb.group({
    password: ['', Validators.required],
    confirm: ['', Validators.required]
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
    if (
      this.form.invalid ||
      this.form.value.password !== this.form.value.confirm ||
      !this.email
    ) return;

    this.loading = true;

    this.auth.resetPassword(this.email, this.form.value.password!)
      .subscribe({
        next: (res:any) => {
          this.loading = false;
          this.toastr.success(res.message);
          this.router.navigate(['/auth/login']);
        },
        error: err => {
          this.loading = false;
          this.toastr.error(err.error?.message);
        }
      });
  }
}
