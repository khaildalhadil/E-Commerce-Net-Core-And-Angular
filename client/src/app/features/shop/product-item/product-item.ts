import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CurrencyPipe } from '@angular/common';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-product-item',
  imports: [MatCardModule, MatButtonModule, CurrencyPipe, MatIcon],
  templateUrl: './product-item.html',
  styleUrl: './product-item.css',
})
export class ProductItem {
  @Input() product?: Product;
}
