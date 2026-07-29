import { Component, inject } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { AccountService } from '../../services/AccountService';

@Component({
  selector: 'app-header',
  imports: [MatIconModule, MatButton, MatBadge, RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  accountService = inject(AccountService);
  private router = inject(Router);

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
