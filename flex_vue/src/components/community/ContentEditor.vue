<template>
    <div class="fg-black" style="--ck-border-radius:0.25rem;">
       <CKEditor :editor="ClassicEditor" v-model="editorData"></ckeditor>
   </div>
</template>

<script setup>
import { ref, watch, defineProps, defineEmits } from 'vue';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { component as CKEditor } from '@ckeditor/ckeditor5-vue';

class UploadAdapter {
    constructor(loader) {
        this.loader = loader;
    }

    upload() {
        return new Promise((resolve, reject) => {
            const reader = new window.FileReader();

            reader.addEventListener('load', () => {
                resolve({ default: reader.result });
            });

            reader.addEventListener('error', (err) => {
                reject(err);
            });

            reader.addEventListener('abort', () => {
                reject();
            });

            this.loader.file.then((file) => {
                reader.readAsDataURL(file);
            });
        });
    }

    abort() {
        // Implement abort logic if needed.
    }
}

const uploader = (editor) => {
    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => new UploadAdapter(loader);
};

const { modelValue } = defineProps();
const emit = defineEmits(['update:modelValue']);
const editorData = ref(modelValue || '');

watch(editorData, () => {
    emit('update:modelValue', editorData.value);
});

const editorConfig = {
    extraPlugins: [uploader],
    toolbar: ['heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', 'image'],
    heading: {
        options: [
            { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
            { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
            { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' }
        ]
    }
};
</script>

<style>
.ck-editor_editable{
    min-height: 500px;
}
</style>
