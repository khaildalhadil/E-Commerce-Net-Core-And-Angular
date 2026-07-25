import { Component, inject } from '@angular/core';
import { ShopService } from '../../../services/ShopService';

@Component({
  selector: 'app-filters-dialog',
  imports: [],
  templateUrl: './filters-dialog.html',
  styleUrl: './filters-dialog.css',
})
export class FiltersDialog {
  shopService = inject(ShopService)
}
