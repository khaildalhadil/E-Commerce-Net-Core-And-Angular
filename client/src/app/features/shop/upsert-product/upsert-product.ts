import { Component, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Product } from '../../../shared/models/product';
import { ShopService } from '../../../services/ShopService';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateProduct } from '../../../shared/models/createProduct';

const DEFAULT_IMAGE_URL = 'https://placehold.co/400x400?text=No+Image';

@Component({
  selector: 'app-upsert-product',
    imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatIconModule],

  templateUrl: './upsert-product.html',
  styleUrl: './upsert-product.css',
})
export class UpsertProduct {

  userDataToUpdate = signal<Product[]>([]);
  imageUrl = signal<string>(DEFAULT_IMAGE_URL);
  isLoading = signal<boolean>(false);
  constructor(
    private shopServie: ShopService,
    private routes: Router,
    private route: ActivatedRoute,
  ) {}

  productForm = new FormGroup({
    name: new FormControl("red shoes", [
      Validators.required,
      Validators.maxLength(40),
      Validators.minLength(2)
    ]),

  description: new FormControl("", [
    Validators.required,
    Validators.maxLength(200),
    Validators.minLength(5)
  ]),

  price: new FormControl(0, [
    Validators.required,
    Validators.min(0)
  ]),

  pictureUrl: new FormControl(DEFAULT_IMAGE_URL, [
    Validators.required
  ]),

  type: new FormControl("", [
    Validators.required,
    Validators.maxLength(30),
    Validators.minLength(2)
  ]),

  brand: new FormControl("", [
    Validators.required,
    Validators.maxLength(30),
    Validators.minLength(2)
  ]),

  quantitiyInStock: new FormControl(0, [
    Validators.required,
    Validators.min(0)
  ])
});

  Save() {

    if (this.productForm.valid) {
      this.shopServie.AddProduct(this.productForm.value as CreateProduct).subscribe({
        next: () => {
          this.routes.navigate(['/']);
        },
        error: (err)=> {
          console.log(err);
        },
        complete() {
        }

      })
    } else {
      console.log(this.productForm.value)
      console.log(this.productForm.valid)
    }
  }

  gerImage() {
    this.isLoading.set(true)
    this.shopServie.GerImage(this.productForm.value.name as string).subscribe({
      next: (imageUrlFromAI) => {
        this.imageUrl.set(imageUrlFromAI);
        this.productForm.patchValue({ pictureUrl: imageUrlFromAI });
      },
      error: (err) => console.log(err),
      complete: () =>{
        this.isLoading.set(false)
      },
    })

  }

  deletImage() {
    this.imageUrl.set(DEFAULT_IMAGE_URL);
    this.productForm.patchValue({ pictureUrl: DEFAULT_IMAGE_URL });
  }

}
