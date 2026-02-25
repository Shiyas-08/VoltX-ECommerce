import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { AuthService, User } from 'src/app/core/services/auth.service';
import { environment } from 'src/environments';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {

  user: User | null = null;
  loading = false;
  api = environment.apiUrl;

  form = this.fb.group({
    name: ['', Validators.required],
    phone: ['', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private http: HttpClient,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    
    this.authService.user$.subscribe(user => {
      if (!user) return;

      this.user = user;
      this.form.patchValue({
        name: user.name,
        phone: user.phone
      });
    });
  }

  save() {
    if (this.form.invalid || this.loading) return;

    this.loading = true;

    this.http.put<any>(
      `${this.api}/auth/update-profile`,
      this.form.value,
      { withCredentials: true }
    ).subscribe({
      next: res => {
        this.loading = false;

        if (res?.data) {
          this.authService.updateUser(res.data);
        }

        this.toastr.success(res.message || 'Profile updated');
      },
      error: err => {
        this.loading = false;
        this.toastr.error(err?.error?.message || 'Update failed');
      }
    });
  }
}
