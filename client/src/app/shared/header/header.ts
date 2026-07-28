import { Component } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-header',
  imports: [MatIconModule, MatButton, MatBadge, RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {}
