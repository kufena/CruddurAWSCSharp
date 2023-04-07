INSERT INTO public.users (email, display_name, handle, cognito_user_id, created_at)
VALUES
  ('fart@old.com','Andrew Douglas', 'andyd', 'MOCK', current_timestamp),
  ('bill@german.org','Andrew Brown', 'andrewbrown', 'MOCK', current_timestamp);

INSERT INTO public.activities (user_uuid, message, expires_at)
VALUES
  (
    (SELECT uuid FROM public.users WHERE users.handle = 'andyd'),
    'This is an imported seed data comment',
    current_timestamp + interval '10 day'
  );