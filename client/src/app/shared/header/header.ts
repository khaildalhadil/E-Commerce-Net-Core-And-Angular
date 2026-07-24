import { Component } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-header',
  imports: [MatIconModule, MatButton, MatBadge],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {}
