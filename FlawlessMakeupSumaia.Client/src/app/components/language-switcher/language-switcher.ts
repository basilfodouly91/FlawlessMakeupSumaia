import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-language-switcher',
    standalone: true,
    imports: [CommonModule, TranslateModule],
    template: `
    <div class="dropdown">
      <button 
        class="btn btn-link nav-link dropdown-toggle" 
        type="button" 
        (click)="toggleDropdown()"
        [attr.aria-expanded]="isOpen">
        {{ currentLang === 'en' ? 'English' : 'العربية' }}
      </button>
      <ul class="dropdown-menu" [class.show]="isOpen">
        <li>
          <button class="dropdown-item" [class.active]="currentLang === 'en'" (click)="switchLanguage('en')">
            English
          </button>
        </li>
        <li>
          <button class="dropdown-item" [class.active]="currentLang === 'ar'" (click)="switchLanguage('ar')">
            العربية
          </button>
        </li>
      </ul>
    </div>
  `,
    styles: [`
    :host {
      display: block;
    }
    .dropdown {
      position: relative;
    }
    .dropdown-menu {
      position: absolute;
      top: 100%;
      left: 0;
      z-index: 1050;
      display: none;
      min-width: 10rem;
      padding: 0.5rem 0;
      margin: 0.125rem 0 0;
      font-size: 1rem;
      color: #212529;
      text-align: left;
      list-style: none;
      background-color: #fff;
      background-clip: padding-box;
      border: 1px solid rgba(0,0,0,.15);
      border-radius: 0.25rem;
      box-shadow: 0 0.5rem 1rem rgba(0,0,0,.175);
    }
    .dropdown-menu.show {
      display: block;
    }
    .dropdown-menu li {
      list-style: none;
    }
    .dropdown-item {
      display: block;
      width: 100%;
      padding: 0.25rem 1rem;
      clear: both;
      font-weight: 400;
      color: #212529;
      text-align: inherit;
      text-decoration: none;
      white-space: nowrap;
      background-color: transparent;
      border: 0;
      cursor: pointer;
    }
    .dropdown-item:hover,
    .dropdown-item:focus {
      color: #1e2125;
      background-color: #e9ecef;
    }
    .dropdown-item.active {
      background-color: var(--pink);
      color: white;
    }
    .dropdown-item.active:hover {
      background-color: var(--pink);
      color: white;
    }
  `]
})
export class LanguageSwitcherComponent implements OnInit {
    currentLang = 'ar';
    isOpen = false;

    constructor(private translate: TranslateService) { }

    ngOnInit() {
        const currentLang = this.translate.currentLang || this.translate.defaultLang || 'ar';
        this.currentLang = currentLang;
        document.documentElement.dir = currentLang === 'ar' ? 'rtl' : 'ltr';
        document.documentElement.lang = currentLang;
    }

    toggleDropdown() {
        this.isOpen = !this.isOpen;
    }

    switchLanguage(lang: string) {
        this.currentLang = lang;
        this.translate.use(lang);
        document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
        document.documentElement.lang = lang;
        this.isOpen = false; // Close dropdown after selection
    }
}
