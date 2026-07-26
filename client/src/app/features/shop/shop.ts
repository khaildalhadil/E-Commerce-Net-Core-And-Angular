import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { Product } from '../../shared/models/product';
import { ProductItem } from './product-item/product-item';
import { ShopService } from '../../services/ShopService';
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialog } from './filters-dialog/filters-dialog';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-shop',
  imports: [ProductItem, MatButton, MatIcon],
  templateUrl: './shop.html',
  styleUrl: './shop.css',
})
export class Shop implements OnInit {

  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  public allProducts = signal<Product[]>([]);

  selectedBrands: string[] = [];
  selectedTypes: string[] = [];
  
  ngOnInit(): void {
    this.GetAllProduct()
    this.initializeShop();
  }

  GetAllProduct() {

    this.shopService.getAllProudcts().subscribe({

      next: (data) => {
        this.allProducts.set(data);
        console.log(data);
      },

      error: (err) => {
        console.log(err);
      },

      complete: () => {
        console.log("Complate")
      }

    })
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
  }

  openFiltersDialog() {
    const dialogRef = this.dialogService.open(FiltersDialog, {
      minWidth: '500px',
      data: {
        selectedBrands: this.selectedBrands,
        selectedTypes: this.selectedTypes,
      }
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if(result) {
          
          
          this.selectedBrands = result.selectedBrands;
          this.selectedTypes = result.selectedTypes;

          this.shopService.getAllProudcts(this.selectedBrands, this.selectedTypes).subscribe({
            next: (data) => console.log(data),
            error: (err) => console.log(err)
          })

          
        }
      },
      error: (er) => {
        console.log(er);
      }
    })
  }
}
