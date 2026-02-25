import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService, User } from 'src/app/core/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { environment } from 'src/environments';

@Component({
  selector: 'app-manage-users',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class ManageUsersComponent implements OnInit, OnDestroy {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading = false;
  searchText = '';
  selectedUser: User | null = null;

  

  private destroy$ = new Subject<void>();
  private userApi = environment.apiUrl+'/User';
  

  constructor(
    private auth: AuthService,
    private http: HttpClient,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

loadUsers(): void {
  this.loading = true;

  this.http
    .get<User[]>(`${this.userApi}/GetAll`, { withCredentials: true })
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: res => {
        this.users = res;
        this.filteredUsers = [...this.users];
        this.loading = false;
      },
      error: () => {
        this.toastr.error('Failed to load users', 'Error');
        this.loading = false;
      }
    });
}


  filterUsers(): void {
    const term = this.searchText.toLowerCase().trim();

    if (!term) {
      this.filteredUsers = [...this.users];
      return;
    }

    this.filteredUsers = this.users.filter(
      u =>
        u.name.toLowerCase().includes(term) ||
        u.email.toLowerCase().includes(term)
    );
  }

  toggleBlock(user: User): void {
  if (!user?.id) return;

  const oldValue = user.isBlocked;
  user.isBlocked = !user.isBlocked; 

  this.http
    .put<any>(
      `${this.userApi}/toggle-block/${user.id}`,
      {},
      { withCredentials: true }
    )
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (res) => {
        this.toastr.success(res.message || 'Status updated');
      },
      error: (err) => {
        user.isBlocked = oldValue;

        this.toastr.error(
          err?.error?.message || 'Failed to update user status'
        );
      }
    });
}

  
  closeModal(): void {
    this.selectedUser = null;
  }
}
