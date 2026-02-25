import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  registerData = {
    name: '',
    email: '',
    phone: '',
    password: ''
  };

  constructor(
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  onRegister(form: any) {
    if (!form.valid) {
      this.toastr.warning('Please fill all fields correctly');
      return;
    }

this.auth.register(this.registerData).subscribe({
  next: () => {
    this.toastr.success('Registration successful. Please login.');
    this.router.navigate(['/login']);
  },
error: (err) => {
  let message = 'Registration failed. Try again';

  if (err?.error?.message) {
    message = err.error.message;
  }
  else if (err?.error?.errors) {
    const firstKey = Object.keys(err.error.errors)[0];
    message = err.error.errors[firstKey][0];
  }

  this.toastr.error(message, 'Registration Error');
}

});

  }
}
